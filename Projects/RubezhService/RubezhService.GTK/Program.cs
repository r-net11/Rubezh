using System;
using System.Threading;
using RubezhService.Views;
using Gtk;

namespace RubezhService
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
			Application.Init();
			var view = new MainView();
			Bootstrapper.Run();
			Application.Run();
		}
	}
}
