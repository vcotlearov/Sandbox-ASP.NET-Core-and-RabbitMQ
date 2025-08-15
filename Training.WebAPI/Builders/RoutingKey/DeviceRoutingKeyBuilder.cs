using Training.WebAPI.Models;

namespace Training.WebAPI.Builders.RoutingKey
{
	public sealed class DeviceRoutingKeyBuilder : RoutingKeyBuilder<Device>
	{
		public override string BuildRoutingKey(Device entity)
		{
			return $"device.{entity.SerialNumber}.state.{entity.State}";
		}
	}
}
