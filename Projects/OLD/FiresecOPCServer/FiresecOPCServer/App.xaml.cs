using System;
using System.Windows;
using Common;
using Infrastructure.Common;
using Infrastructure.Common.Theme;

namespace FiresecOPCServer
{
	public partial class App : Application
	{
		private const string SignalId = "148996BE-2E10-40B3-9D8A-07CD57A2847F";
		private const string WaitId = "9D2ADF91-2C69-493B-9809-C1F2851D4ECD";

		private void OnStartup(object sender, StartupEventArgs e)
		{
			try
			{
				PatchManager.Patch();
                ThemeHelper.LoadThemeFromRegister();
				AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
				using (new DoubleLaunchLocker(SignalId, WaitId, true))
				{
					try
					{
						Bootstrapper.Run();
					}
					catch (Exception ex)
					{
						Logger.Error(ex, "App.OnStartup 1");
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error(ex, "App.OnStartup 2");
			}
		}

		void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Logger.Error(e.ExceptionObject as Exception);
		}
	}
}