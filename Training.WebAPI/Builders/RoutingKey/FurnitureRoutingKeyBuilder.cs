using Training.WebAPI.Models;

namespace Training.WebAPI.Builders.RoutingKey
{
	public sealed class FurnitureRoutingKeyBuilder : RoutingKeyBuilder<Furniture>
	{
		public override string BuildRoutingKey(Furniture entity)
		{
			return $"furniture.{entity.SerialNumber}.state.InUse";
		}
	}
}
