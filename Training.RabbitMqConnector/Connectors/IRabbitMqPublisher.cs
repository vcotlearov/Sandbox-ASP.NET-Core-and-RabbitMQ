namespace Training.RabbitMqConnector.Connectors
{
	public interface IRabbitMqPublisher : IAsyncDisposable
	{
		Task<bool> PushAsync(string key, ReadOnlyMemory<byte> message);

		Task<bool> PullAsync(Func<string, Task> onMessageReceived, CancellationToken cancellationToken);
	}
}
