using Controls;
using Infrastructure;
using System;
using Shell = FireMonitor;

namespace FireMonitor.Layout
{
	public class App : Shell.App
	{
		protected override Shell.Bootstrapper CreateBootstrapper()
		{
			return new Bootstrapper();
		}

		[STAThread]
		static void Main()
		{
			ServiceFactory.StartupService.Run();
			var app = new App();
			ServiceFactory.ResourceService.AddResource(typeof(UIBehavior).Assembly, "Themes/Styles.xaml");
			app.Run();
		}
	}
}