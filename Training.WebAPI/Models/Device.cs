using System.Text.Json.Serialization;

namespace Training.WebAPI.Models
{
	public class Device
	{
		[JsonPropertyName("serial_number")]
		public required string SerialNumber { get; set; }
		
		[JsonPropertyName("name")]
		public required string Name { get; set; }
		
		[JsonPropertyName("online")]
		public required bool Online { get; set; }

		public string State => Online ? "online" : "offline";
	}
}
