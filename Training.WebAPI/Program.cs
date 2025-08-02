using System.Text.Json.Serialization.Metadata;
using Training.RabbitMqConnector.Extensions;
using Training.RabbitMqConnector.Models.Options;
using Training.WebAPI.Builders.RoutingKey;
using Training.WebAPI.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMqOptions"));
builder.Services.AddRabbitMqPublisher();

builder.Services.AddSingleton<RoutingKeyBuilderDirector>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
	options.JsonSerializerOptions.TypeInfoResolver = JsonTypeInfoResolver.Combine(
		WebApiJsonContext.Default,
		FrameworkJsonContext.Default
	); ;
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
