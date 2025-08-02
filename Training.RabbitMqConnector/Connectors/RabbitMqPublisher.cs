using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;
using Training.RabbitMqConnector.Models.Options;

namespace Training.RabbitMqConnector.Connectors;
public class RabbitMqPublisher : IRabbitMqPublisher
{
	private readonly IConnection _connection;
	private readonly IChannel _channel;
	private readonly string _exchangeName;
	private readonly string _queueName;

	private RabbitMqPublisher(IConnection connection, IChannel channel, string exchangeName, string queueName)
	{
		_connection = connection;
		_channel = channel;
		_exchangeName = exchangeName;
		_queueName = queueName;
	}

	public static async Task<RabbitMqPublisher> CreateAsync(IOptions<RabbitMqOptions> options)
	{
		var opt = options.Value;
		var factory = new ConnectionFactory()
		{
			HostName = opt.HostName,
			Port = opt.Port,
			UserName = opt.UserName,
			Password = opt.Password,
			VirtualHost = opt.VirtualHost,
			AutomaticRecoveryEnabled = opt.AutomaticRecoveryEnabled
		};

		var connection = await factory.CreateConnectionAsync();
		var channel = await connection.CreateChannelAsync();

		await channel.QueueDeclareAsync(
			queue: opt.QueueName,
			durable: true,
			exclusive: false,
			autoDelete: false,
			arguments: null);

		return new RabbitMqPublisher(connection, channel, opt.ExchangeName, opt.QueueName);
	}

	public async Task PushAsync(string key, string message)
	{
		var body = Encoding.UTF8.GetBytes(message);

		await _channel.QueueBindAsync(
			queue: _queueName,
			exchange: _exchangeName,
			routingKey: key);

		await _channel.BasicPublishAsync(
			exchange: _exchangeName,
			routingKey: key,
			body: body);
	}

	public async Task PullAsync(Func<string, Task> onMessageReceived, CancellationToken cancellationToken)
	{
		var consumer = new AsyncEventingBasicConsumer(_channel);
		consumer.ReceivedAsync += async (model, ea) =>
		{
			var body = ea.Body.ToArray();
			var message = Encoding.UTF8.GetString(body);
			await onMessageReceived(message);  // User provided callback to process the message
		};

		await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: cancellationToken);

		var consume = await _channel.BasicConsumeAsync(_queueName, autoAck: true, consumer: consumer, cancellationToken: cancellationToken);

		try
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				await Task.Delay(500, cancellationToken);
			}
		}
		catch (TaskCanceledException)
		{
		}

		await _channel.BasicCancelAsync(consume, cancellationToken: cancellationToken);
	}

	public async ValueTask DisposeAsync()
	{
		await _connection.DisposeAsync();
		await _channel.DisposeAsync();
	}
}