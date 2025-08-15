namespace Training.WebAPI.Builders.RoutingKey
{
	public abstract class RoutingKeyBuilder<T>: IRoutingKeyBuilder
	{
		public bool CanBuild(object entity)
		{
			return entity is T;
		}

		public string BuildRoutingKey(object entity)
		{
			return BuildRoutingKey((T)entity);
		}

		public abstract string BuildRoutingKey(T entity);
	}
}