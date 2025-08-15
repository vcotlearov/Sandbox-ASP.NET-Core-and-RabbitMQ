using System.Text.Json.Serialization;
using Training.WebAPI.Models;

namespace Training.WebAPI.Serialization;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(Furniture))]
public partial class FurnitureJsonContext : JsonSerializerContext
{
}