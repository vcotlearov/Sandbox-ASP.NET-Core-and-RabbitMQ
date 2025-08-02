using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Training.RabbitMqConnector.Connectors;
using Training.WebAPI.Builders.RoutingKey;
using Training.WebAPI.Models;

namespace Training.WebAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class DeviceController(ILogger<DeviceController> logger, IRabbitMqPublisher publisher, RoutingKeyBuilderDirector routingKeyBuilder) : Controller
	{
		private readonly ILogger<DeviceController> _logger = logger;

		[HttpPost]
		[Route("[action]")]
		public async Task<IActionResult> SwitchState(Device device)
		{
			await publisher.PushAsync(routingKeyBuilder.BuildRoutingKey(device), JsonSerializer.Serialize(device));
			return Ok("Message sent");
		}
	}
}
