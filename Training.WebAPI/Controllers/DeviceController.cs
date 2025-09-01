using Microsoft.AspNetCore.Mvc;
using System.Buffers;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Training.RabbitMqConnector.Connectors;
using Training.WebAPI.Builders.RoutingKey;
using Training.WebAPI.Models;
using Training.WebAPI.Serialization;

namespace Training.WebAPI.Controllers
{
	[ApiController]
	[Route("Device")]
	public class DeviceController(ILogger<DeviceController> logger, IRabbitMqPublisher publisher, RoutingKeyBuilderDirector routingKeyBuilder) : Controller
	{
		private const int DeviceLargestPayloadSize = 129;
		private const int FurnitureLargestPayloadSize = 108;
		private static async Task<ReadOnlyMemory<byte>> ConvertToReadOnlyMemoryAsync(IModel entity, byte[] buffer, JsonTypeInfo dsd)
		{
			using var ms = new MemoryStream(buffer, writable: true);
			await JsonSerializer.SerializeAsync(ms, entity, dsd);
			ReadOnlyMemory<byte> payload = buffer.AsMemory(0, (int)ms.Length);
			return payload;
		}

		[HttpPost]
		[Route("SwitchState")]
		public async Task<IActionResult> SwitchState(Device entity)
		{
			var buffer = ArrayPool<byte>.Shared.Rent(DeviceLargestPayloadSize);
			try
			{
				var key = routingKeyBuilder.BuildRoutingKey(entity);

				logger.LogInformation($"Invoking SwitchState for device SN '{entity.SerialNumber}'");
				logger.LogInformation($"Using routing key: '{key}'");
				logger.LogInformation("Sending message to message broker");

				var payload = await ConvertToReadOnlyMemoryAsync(entity, buffer, DeviceJsonContext.Default.Device);

				var result = await publisher.PushAsync(key, payload);
				var responseText = result ? "Message sent" : "Message could not be sent. Check log for details";
				logger.LogInformation(responseText);

				return Ok(responseText);
			}
			finally
			{
				ArrayPool<byte>.Shared.Return(buffer);
			}
		}

		[HttpPost]
		[Route("FurnitureCheck")]
		public async Task<IActionResult> FurnitureCheck(Furniture entity)
		{
			var buffer = ArrayPool<byte>.Shared.Rent(FurnitureLargestPayloadSize);
			try
			{
				var key = routingKeyBuilder.BuildRoutingKey(entity);

				logger.LogInformation($"Invoking FurnitureCheck for piece '{entity}'");
				logger.LogInformation($"Using routing key: '{key}'");
				logger.LogInformation("Sending message to message broker");

				var payload = await ConvertToReadOnlyMemoryAsync(entity, buffer, FurnitureJsonContext.Default.Furniture);

				var result = await publisher.PushAsync(key, payload);
				var responseText = result ? "Message sent" : "Message could not be sent. Check log for details";
				logger.LogInformation(responseText);

				return Ok(responseText);
			}
			finally
			{
				ArrayPool<byte>.Shared.Return(buffer);
			}
		}
	}
}