using System;
using System.Diagnostics;
using System.Windows;
using Common;
using FiresecService;
using Infrastructure.Common;
using Infrastructure.Common.BalloonTrayTip;
using Infrastructure.Common.Theme;
using Infrastructure.Automation;

namespace FiresecService
{
	class MainClass
	{
		private const string SignalId = "{9C3B6318-48BB-40D0-9249-CA7D9365CDA5}";
		private const string WaitId = "{254FBDB4-7632-42A8-B2C2-27176EF7E60C}";

		public static void Main (string[] args)
		{
			Console.WriteLine ("Starting...");

			ServerLoadHelper.SetLocation(System.Reflection.Assembly.GetExecutingAssembly().Location);
			ServerLoadHelper.SetStatus(FSServerState.Opening);

			using (new DoubleLaunchLocker(SignalId, WaitId, true))
			{
				AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
				try
				{
					Bootstrapper.Run();
				}
				catch (Exception ex)
				{
					Logger.Error(ex, "App.OnStartup");
					BalloonHelper.ShowFromServer("Ошибка во время загрузки");
					return;
				}
			}
		}

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Logger.Error((Exception)e.ExceptionObject, "App.CurrentDomain_UnhandledException");
			Bootstrapper.Close();
		}
	}
}

