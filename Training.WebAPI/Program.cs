using Serilog;
using System.Text.Json.Serialization.Metadata;
using Training.RabbitMqConnector.Extensions;
using Training.RabbitMqConnector.Models.Options;
using Training.WebAPI.Builders.RoutingKey;
using Training.WebAPI.Serialization;

Log.Logger = new LoggerConfiguration()
	.Enrich.FromLogContext()
	.WriteTo.Console()
	.CreateBootstrapLogger();

try
{
	var builder = WebApplication.CreateBuilder(args);
	builder.Services.AddSerilog((services, lc) => lc
		.ReadFrom.Configuration(builder.Configuration)
		.ReadFrom.Services(services)
		.Enrich.FromLogContext()
		.WriteTo.Console());

	Log.Information("Starting web application");

	builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMqOptions"));
	builder.Services.AddRabbitMqPublisher();
	builder.Services.AddSingleton<RoutingKeyBuilderDirector>();
	builder.Services.AddControllers().AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.TypeInfoResolver = JsonTypeInfoResolver.Combine(
			WebApiJsonContext.Default,
			FrameworkJsonContext.Default
		);
	});

	builder.Services.AddLogging(logging =>
	{
		logging.ClearProviders();
		logging.AddConsole();
	});

	var app = builder.Build();
	app.MapControllers();
	app.Run();
}
catch (Exception ex)
{
	Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
	Log.CloseAndFlush();
}