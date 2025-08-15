namespace Training.Console.Sinks
{
	public interface ISink
	{
		void PrintHeader(string message);
		void Print(string message);
		Task DisplayStatusAsync(Task wrapper);
		Task PrintMessage(string message);
		void PrintFarewell(string message);
		bool ReadKey();
	}
}