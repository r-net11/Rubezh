using System.Windows;
using Infrastructure.Common;
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
			RegistrySettingsHelper.SetString("GKImitatorPath", System.Reflection.Assembly.GetExecutingAssembly().Location);
			Bootstrapper.Run();
		}
	}
}