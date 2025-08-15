using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Training.RabbitMqConnector.Connectors;
using Training.WebAPI.Builders.RoutingKey;
using Training.WebAPI.Models;

namespace Training.WebAPI.Controllers
{
	[ApiController]
	[Route("Device")]
	public class DeviceController(ILogger<DeviceController> logger, IRabbitMqPublisher publisher, RoutingKeyBuilderDirector routingKeyBuilder) : Controller
	{
		[HttpPost]
		[Route("SwitchState")]
		public async Task<IActionResult> SwitchState(Device device)
		{
			var message = JsonSerializer.Serialize(device);
			var key = routingKeyBuilder.BuildRoutingKey(device);

			logger.LogInformation($"Invoking SwitchState for device '{message}'");
			logger.LogInformation($"Using routing key: '{key}'");
			logger.LogInformation("Sending message to message broker");
			
			var result = await publisher.PushAsync(key, message);
			var responseText = result ? "Message sent" : "Message could not be sent. Check log for details";
			logger.LogInformation(responseText);

			return Ok(responseText);
		}

		[HttpPost]
		[Route("FurnitureCheck")]
		public async Task<IActionResult> FurnitureCheck(Furniture entity)
		{
			var message = JsonSerializer.Serialize(entity);
			var key = routingKeyBuilder.BuildRoutingKey(entity);

			logger.LogInformation($"Invoking FurnitureCheck for piece '{message}'");
			logger.LogInformation($"Using routing key: '{key}'");
			logger.LogInformation("Sending message to message broker");

			var result = await publisher.PushAsync(key, message);
			var responseText = result ? "Message sent" : "Message could not be sent. Check log for details";
			logger.LogInformation(responseText);

			return Ok(responseText);
		}
	}
}