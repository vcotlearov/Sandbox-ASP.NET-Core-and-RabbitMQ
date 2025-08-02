namespace Training.WebAPI.Builders.RoutingKey
{
	public interface IRoutingKeyBuilder
	{
		bool CanBuild(object entity);
		string BuildRoutingKey(object entity);
	}
}
