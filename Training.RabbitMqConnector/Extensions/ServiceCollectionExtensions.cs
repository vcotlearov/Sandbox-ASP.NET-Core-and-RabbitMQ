using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Training.RabbitMqConnector.Connectors;
using Training.RabbitMqConnector.Models.Options;

namespace Training.RabbitMqConnector.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddRabbitMqPublisher(this IServiceCollection services)
		{
			services.AddSingleton<IRabbitMqPublisher>(sp =>
			{
				var options = sp.GetRequiredService<IOptions<RabbitMqOptions>>();
				return RabbitMqPublisher.CreateAsync(options).GetAwaiter().GetResult();
			});

			return services;
		}
	}
}
