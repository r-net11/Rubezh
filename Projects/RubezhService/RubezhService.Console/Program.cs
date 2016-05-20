
using System;
using System.Threading.Tasks;
namespace RubezhService
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.CursorVisible = false;
			Console.CancelKeyPress += Console_CancelKeyPress;
			PageController.Redraw();
			
			new Task(() => Bootstrapper.Run()).Start();
						
			while (true)
			{
				var key = Console.ReadKey();
				PageController.OnKeyPressed(key.Key);
			}
		}

		static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
		{
			Environment.Exit(1);
		}
	}
}
