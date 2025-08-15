namespace Training.WebAPI.Builders.RoutingKey
{
	public class RoutingKeyBuilderDirector
	{
		private readonly List<IRoutingKeyBuilder> _builders =
		[
			new DeviceRoutingKeyBuilder(),
			new FurnitureRoutingKeyBuilder()
		];

		public string BuildRoutingKey(object entity)
		{
			var builder = _builders.FirstOrDefault(b => b.CanBuild(entity));
			if (builder == null)
				throw new InvalidOperationException($"No routing key builder registered for entity type {entity.GetType().Name}");

			return builder.BuildRoutingKey(entity);
		}
	}
}
