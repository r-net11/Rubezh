using Shell = FireMonitor;
using System;
using Infrastructure;
using System.Windows;
using Infrastructure.Common;
using Controls;
using System.Windows.Shapes;

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
			var resourceLocater = new ResourceDescription(typeof(UIBehavior).Assembly, "Themes/Styles.xaml");
			app.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = resourceLocater.Source });
			app.Resources.Add(typeof(Rectangle), new Style(typeof(Rectangle)));
			app.Run();
		}
	}
}
