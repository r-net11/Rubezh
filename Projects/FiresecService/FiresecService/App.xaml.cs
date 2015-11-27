using System.Threading;
using System.Windows.Threading;
using Common;
using FiresecService;
using FiresecService.ViewModels;
using FiresecService.Views;
using Infrastructure.Common;
using Infrastructure.Common.BalloonTrayTip;
using Infrastructure.Common.Theme;
using System;
using System.Diagnostics;
using System.Windows;
using KeyGenerator;

namespace FiresecServiceRunner
{
	public partial class App : Application
	{
		private const string SignalId = "{9C3B6318-48BB-40D0-9249-CA7D9365CDA5}";
		private const string WaitId = "{254FBDB4-7632-42A8-B2C2-27176EF7E60C}";

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			ThemeHelper.LoadThemeFromRegister();
			ServerLoadHelper.SetLocation(System.Reflection.Assembly.GetExecutingAssembly().Location);
			ServerLoadHelper.SetStatus(FSServerState.Opening);
			string prodKey = null;
			try
			{
				prodKey = Generator.Load(Generator.GetProcessorID(), AppDataFolderHelper.GetFile("LicData.dat"));
			}
			catch (Exception ex)
			{
				Logger.Error(ex, "Исключение при вызове Generator.Load");
			}
			if (!Generator.VerifyProductKey(prodKey))
			{
				// Create a thread
				var newWindowThread = new Thread(() =>
				{
					// Create our context, and install it:
					SynchronizationContext.SetSynchronizationContext(
						new DispatcherSynchronizationContext(
							Dispatcher.CurrentDispatcher));
					// Create and show the Window
					var tempWindow = new RegistrationWindow {DataContext = new RegistrationViewModel()};
					tempWindow.Closed += (s, er) => Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
					tempWindow.Show();
					// Start the Dispatcher Processing
					System.Windows.Threading.Dispatcher.Run();
				});
				// Set the apartment state
				newWindowThread.SetApartmentState(ApartmentState.STA);
				// Make the thread a background thread
				newWindowThread.IsBackground = true;
				// Start the thread
				newWindowThread.Start();
			}
			else
			{
				using (new DoubleLaunchLocker(SignalId, WaitId, true))
				{
					AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
					AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(CurrentDomain_AssemblyLoad);
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
		}

		protected override void OnExit(ExitEventArgs e)
		{
			ProcedureRunner.Terminate();
			base.OnExit(e);
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Logger.Error((Exception)e.ExceptionObject, "App.CurrentDomain_UnhandledException");
			BalloonHelper.ShowFromServer("Перезагрузка");
			var processStartInfo = new ProcessStartInfo()
			{
				FileName = Application.ResourceAssembly.Location
			};
			System.Diagnostics.Process.Start(processStartInfo);
			Bootstrapper.Close();
			Application.Current.MainWindow.Close();
			Application.Current.Shutdown();
		}

		private void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
		{
		}
	}
}