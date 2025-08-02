namespace Training.RabbitMqConnector.Connectors
{
	public interface IRabbitMqPublisher : IAsyncDisposable
	{
		Task PushAsync(string key, string message);

		Task PullAsync(Func<string, Task> onMessageReceived, CancellationToken cancellationToken);
	}
}
