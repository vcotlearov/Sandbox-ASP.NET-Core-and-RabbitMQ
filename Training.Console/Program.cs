using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Spectre.Console;
using Spectre.Console.Json;
using Training.Console;
using Training.RabbitMqConnector.Connectors;
using Training.RabbitMqConnector.Extensions;
using Training.RabbitMqConnector.Models.Options;

Log.Logger = new LoggerConfiguration()
	.WriteTo.File("logs/log-.log",
		rollingInterval: RollingInterval.Day,
		retainedFileCountLimit: 7)
	.CreateLogger();

using var host = Host.CreateDefaultBuilder(args)
	.UseSerilog()
	.ConfigureServices((context, services) =>
	{
		services.Configure<RabbitMqOptions>(context.Configuration.GetSection("RabbitMqOptions"));
		services.AddRabbitMqPublisher();
	})
	.Build();

var publisher = host.Services.GetRequiredService<IRabbitMqPublisher>();

Log.Information("Starting console application");

PrintConsoleHeader();

var cts = SetCancellationToken();

try
{
	await AnsiConsole.Status().StartAsync("[yellow dim]Listening...[/]", async ctx =>
	{
		ctx.Spinner(
			new CustomDotsSpinner());
		ctx.SpinnerStyle(Style.Parse("yellow dim"));
		await publisher.PullAsync(async (msg) =>
		{
			Log.Information($"Message received: {msg}");

			AnsiConsole.MarkupLine($"[[{DateTime.Now:G}]] \t:bell:");
			AnsiConsole.Write(
				new JsonText(msg)
					.BracesColor(Color.Red)
					.BracketColor(Color.Green)
					.ColonColor(Color.Blue)
					.CommaColor(Color.Red)
					.StringColor(Color.Green)
					.NumberColor(Color.Blue)
					.BooleanColor(Color.Red)
					.NullColor(Color.Green));
			AnsiConsole.WriteLine();

			await Task.CompletedTask;
		}, cts.Token);
	});
}
catch (TaskCanceledException)
{
	Log.Information($"Connection closed");

	AnsiConsole.MarkupLine("[white dim]Connection closed[/]");
	AnsiConsole.MarkupLine("[white]Press any key to close the console[/]");
	Console.ReadKey();
}
await publisher.DisposeAsync();
return;

void PrintConsoleHeader()
{
	Console.OutputEncoding = System.Text.Encoding.UTF8;

	AnsiConsole.Write(new Align(new Markup("[grey]Press Ctrl+C to stop.[/]\n"), HorizontalAlignment.Right, VerticalAlignment.Top));
	var rule = new Rule("[bold]Events[/]");
	AnsiConsole.Write(rule);
}

CancellationTokenSource SetCancellationToken()
{
	var cancellationTokenSource = new CancellationTokenSource();
	Console.CancelKeyPress += (s, e) =>
	{
		e.Cancel = true;
		cancellationTokenSource.Cancel();
		Log.Information($"Closing connection to the RabbitMQ");
		AnsiConsole.MarkupLine("[white dim]Closing connection...[/]");
	};
	return cancellationTokenSource;
}