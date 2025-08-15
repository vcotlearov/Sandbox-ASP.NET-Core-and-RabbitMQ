using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Json;

namespace Training.Console.Sinks
{
	public class SpectreConsoleSink(IAnsiConsole console, ILogger<SpectreConsoleSink> logger) : ISink
	{
		public void PrintHeader(string message)
		{
			console.Write(new Align(new Markup("[grey]Press Ctrl+C to stop.[/]\n"), HorizontalAlignment.Right, VerticalAlignment.Top));
			var rule = new Rule($"[bold]{message}[/]");
			console.Write(rule);
		}

		public void Print(string message)
		{
			console.Write(message);
		}

		public bool ReadKey()
		{
			var ff=console.Input.ReadKey(false);
			return true;
		}

		public async Task DisplayStatusAsync(Task wrapper)
		{
			await console
				.Status()
				.Spinner(new CustomDotsSpinner())
				.SpinnerStyle(Style.Parse("yellow dim"))
				.StartAsync("[yellow dim]Listening...[/]", _ => wrapper);
		}

		public Task PrintMessage(string message)
		{
			logger.LogInformation($"Message received: {message}");

			console.MarkupLine($"[[{DateTime.Now:G}]] \t:bell:");
			console.Write(
				new JsonText(message)
					.BracesColor(Color.Red)
					.BracketColor(Color.Green)
					.ColonColor(Color.Blue)
					.CommaColor(Color.Red)
					.StringColor(Color.Green)
					.NumberColor(Color.Blue)
					.BooleanColor(Color.Red)
					.NullColor(Color.Green));
			console.WriteLine();
			
			return Task.CompletedTask;
		}

		public void PrintFarewell(string message)
		{
			logger.LogInformation("Connection closed");

			console.MarkupLine($"[white dim]{message}[/]");
			console.MarkupLine("[white]Press any key to close the console[/]");
		}
	}
}