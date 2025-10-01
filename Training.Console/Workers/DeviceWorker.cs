using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Training.Console.Sinks;
using Training.RabbitMqConnector.Connectors;

namespace Training.Console.Workers
{
	public class DeviceWorker(IRabbitMqPublisher publisher, ISink sink, ILogger<DeviceWorker> logger) : IHostedService
	{
		public async Task StartAsync(CancellationToken cancellationToken)
		{
			logger.LogInformation("Starting console application");

			//sink.Print("Press any key to boot the console");
			//sink.ReadKey();
			
			sink.PrintHeader("Events");

			await sink.DisplayStatusAsync(
				publisher.PullAsync(async (msg) =>
							{
								await sink.PrintMessage(msg);
								await Task.CompletedTask;
							}, cancellationToken));
		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			await publisher.DisposeAsync();

			sink.PrintFarewell("Connection closed");
		}
	}
}
