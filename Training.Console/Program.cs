using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Spectre.Console;
using Training.Console.Sinks;
using Training.Console.Workers;
using Training.RabbitMqConnector.Connectors;
using Training.RabbitMqConnector.Models.Options;

var builder = Host.CreateApplicationBuilder(args);

Log.Logger = new LoggerConfiguration()
	.WriteTo.File("logs/log-.log",
		rollingInterval: RollingInterval.Day,
		retainedFileCountLimit: 7)
	.CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(Log.Logger, dispose: true);

builder.Services.AddSingleton<IAnsiConsole>(_ => AnsiConsole.Console);
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMqOptions"));
builder.Services.AddHostedService<DeviceWorker>();

builder.Services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();
builder.Services.AddScoped<ISink, SpectreConsoleSink>();

var app = builder.Build();

try
{
	Console.OutputEncoding = System.Text.Encoding.UTF8;
	
	var cts = SetCancellationToken();
	await app.RunAsync(cts.Token);
}
catch (Exception ex)
{
	Log.Fatal(ex, "Application failed to start correctly");
}
finally
{
	Log.CloseAndFlush();
}

return;

CancellationTokenSource SetCancellationToken()
{
	var cancellationTokenSource = new CancellationTokenSource();
	Console.CancelKeyPress += (s, e) =>
	{
		e.Cancel = true;
		cancellationTokenSource.Cancel();
	};
	return cancellationTokenSource;
}