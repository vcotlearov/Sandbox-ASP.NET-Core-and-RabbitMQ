using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace Training.WebAPI.Serialization;

[JsonSerializable(typeof(ProblemDetails))]
[JsonSerializable(typeof(ValidationProblemDetails))]
[JsonSerializable(typeof(Dictionary<string, string[]>))]
[JsonSerializable(typeof(Dictionary<string, object?>))]
public partial class FrameworkJsonContext : JsonSerializerContext
{
}