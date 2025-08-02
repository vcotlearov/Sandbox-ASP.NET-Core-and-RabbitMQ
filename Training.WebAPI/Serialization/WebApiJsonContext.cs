using System.Text.Json.Serialization;
using Training.WebAPI.Models;

namespace Training.WebAPI.Serialization;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(Device))]
public partial class WebApiJsonContext : JsonSerializerContext
{
}