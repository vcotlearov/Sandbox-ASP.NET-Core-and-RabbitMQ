using Spectre.Console;

namespace Training.Console
{
	internal class CustomDotsSpinner : Spinner
	{
		public override TimeSpan Interval => TimeSpan.FromMilliseconds(80);
		public override bool IsUnicode => true;

		public override IReadOnlyList<string> Frames =>
			new List<string>
				{ "⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏" };
	}
}
