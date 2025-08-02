using Training.WebAPI.Models;

namespace Training.WebAPI.Builders.RoutingKey
{
	public class DeviceRoutingKeyBuilder : IRoutingKeyBuilder
	{
		public bool CanBuild(object entity) => entity is Device;

		public string BuildRoutingKey(object entity)
		{
			var device = (Device)entity;
			return $"device.{device.SerialNumber}.state.{device.State}";
		}
	}
}
