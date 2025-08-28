namespace Training.WebAPI.Builders.RoutingKey
{
	public class RoutingKeyBuilderDirector(IEnumerable<IRoutingKeyBuilder> builders)
	{
		public string BuildRoutingKey(object entity)
		{
			var builder = builders.FirstOrDefault(b => b.CanBuild(entity));
			if (builder == null)
				throw new InvalidOperationException($"No routing key builder registered for entity type {entity.GetType().Name}");

			return builder.BuildRoutingKey(entity);
		}
	}
}
