using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Training.WebAPI.Models
{
	public class Device : IModel
	{
		[JsonPropertyName("serial_number")]
		[Required(ErrorMessage = "SerialNumber is required")]
		[StringLength(36, ErrorMessage = "Serial number must be at most 36 characters")]
		[RegularExpression(@"^[a-zA-Z0-9\-]*$", ErrorMessage = "Serial number can only contain alphanumeric characters and hyphens")]
		public string SerialNumber { get; set; } = null!;

		[JsonPropertyName("name")]
		[Required(ErrorMessage = "Name is required")]
		[StringLength(30, ErrorMessage = "Name can be at most 30 characters long")]
		public string Name { get; set; } = null!;

		[JsonPropertyName("online")]
		[Required(ErrorMessage = "Online status is required")]
		public bool Online { get; set; }

		public string State => Online ? "online" : "offline";
	}
}
