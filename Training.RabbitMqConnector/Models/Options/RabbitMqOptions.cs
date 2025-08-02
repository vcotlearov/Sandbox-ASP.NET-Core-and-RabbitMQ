﻿namespace Training.RabbitMqConnector.Models.Options
{
	public class RabbitMqOptions
	{
		public required string HostName { get; set; }
		public required int Port { get; set; }
		public required string UserName { get; set; }
		public required string Password { get; set; }
		public required string VirtualHost { get; set; }
		public bool AutomaticRecoveryEnabled { get; set; }
		public required string ExchangeName { get; set; }
		public required string QueueName { get; set; }
	}
}
