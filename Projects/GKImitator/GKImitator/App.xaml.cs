using System.Windows;
using Infrastructure.Common.Theme;
using GKImitator.Processor;

namespace GKImitator
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			ThemeHelper.LoadThemeFromRegister();
			Bootstrapper.Run();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			DBHelper.Save();
			base.OnExit(e);
		}
	}
}