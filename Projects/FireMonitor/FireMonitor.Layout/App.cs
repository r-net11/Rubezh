using System;
using System.Windows;
using System.Windows.Shapes;
using Controls;
using Infrastructure;
using Infrastructure.Common.Windows;
using Shell = FireMonitor;

namespace FireMonitor.Layout
{
	public partial class App : Shell.App
	{
		protected override Shell.Bootstrapper CreateBootstrapper()
		{
			return new Bootstrapper();
		}

		[STAThread]
		private static void Main()
		{
			ServiceFactory.StartupService.Run();
			var app = new App();
			ServiceFactory.ResourceService.AddResource(typeof(UIBehavior).Assembly, "Themes/Styles.xaml");
			app.Run();
		}
	}
}