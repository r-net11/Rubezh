using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ResursTerminal.Forms;

namespace ResursTerminal
{
	//class Program
	//{
	//	static void Main(string[] args)
	//	{
	//		Console.WriteLine("Старт");




	//		Console.WriteLine("Для завершения работы нажми ENTER...");
	//		Console.ReadLine();
	//	}
	//}

	static class Program
	{
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new FormMain());
		}
	}
}
