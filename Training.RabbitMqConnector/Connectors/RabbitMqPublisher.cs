using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Text;
using Training.RabbitMqConnector.Models.Options;

namespace Training.RabbitMqConnector.Connectors;
public class RabbitMqPublisher(IOptions<RabbitMqOptions> options, ILogger<RabbitMqPublisher> logger) : IRabbitMqPublisher
{
	private IConnection? _connection;
	private IChannel? _channel;
	private string _exchangeName = string.Empty;
	private string _queueName = string.Empty;

	private async Task<bool> ConnectAsync()
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

		IConnection? connection = null;
		try
		{
			connection = await factory.CreateConnectionAsync();
			logger.LogInformation($"Established connection with RabbitMQ service at '{opt.HostName}:{opt.Port}' using account '{opt.UserName}'");
		}
		catch (BrokerUnreachableException ex)
		{
			logger.LogCritical($"Unable to establish connection with RabbitMQ service. \n Details: {ex}");
		}

		if (connection is null)
			return false;

		var channel = await connection.CreateChannelAsync();

		await channel.QueueDeclareAsync(
			queue: opt.QueueName,
			durable: true,
			exclusive: false,
			autoDelete: false,
			arguments: null);

		_connection = connection;
		_channel = channel;
		_exchangeName = opt.ExchangeName;
		_queueName = opt.QueueName;

		return true;
	}

	public async Task<bool> PushAsync(string key, ReadOnlyMemory<byte> message)
	{
		if (_channel is null)
		{
			if (!await ConnectAsync())
			{
				logger.LogError("Cannot push a message. No Channel is defined");
				return false;
			}
		}

		await _channel.QueueBindAsync(
			queue: _queueName,
			exchange: _exchangeName,
			routingKey: key);

		await _channel.BasicPublishAsync(
			exchange: _exchangeName,
			routingKey: key,
			body: message);

		return true;
	}

	public async Task<bool> PullAsync(Func<string, Task> onMessageReceived, CancellationToken cancellationToken)
	{
		if (_channel is null)
		{
			if (!await ConnectAsync())
			{
				logger.LogError("Cannot pull a message. No Channel is defined");
				return false;
			}
		}

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

		using var cancelTimeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

		try
		{
			await _channel.BasicCancelAsync(consume, true, cancelTimeoutCts.Token); // this one requires a separate cancellation token to ensure it is not outright dismissed
		}
		catch (TaskCanceledException)
		{
		}

		
		return true;
	}

	public async ValueTask DisposeAsync()
	{
		if (_connection != null) await _connection.DisposeAsync();
		if (_channel != null) await _channel.DisposeAsync();
	}
}