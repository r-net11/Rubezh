using System;
using System.Threading;
using FiresecService.Views;
using Gtk;

namespace FiresecService
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
