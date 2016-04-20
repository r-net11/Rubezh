using System.Windows;
using Infrastructure.Common.Windows.Theme;

namespace ItvIntegration
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			ThemeHelper.LoadThemeFromRegister();
		}
	}
}