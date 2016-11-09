using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using Common;
using Localization.FireMonitor.Common;
using Localization.FireMonitor.Errors;
using StrazhAPI.Enums;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Theme;
using Infrastructure.Common.Windows;
using System.Threading;
using Infrastructure.Client.Startup;
using System.Globalization;
using WinForms = System.Windows.Forms;
using System.Windows.Forms;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace FireMonitor
{
	public partial class App : Application
	{
		private static readonly InterceptKeys.LowLevelKeyboardProc _proc = HookCallback;
		private static IntPtr _hookID = IntPtr.Zero;

		private const string SignalId = "{56E74882-C633-45CD-BE94-DCDEE52846BE}";
		private const string WaitId = "{38A602C9-D6F2-424B-B0FA-97E1B133C473}";
		private Bootstrapper _bootstrapper;
		public bool IsClosingOnException { get; private set; }

		public App()
		{
			IsClosingOnException = false;

			if (UserShellHelper.GetShell() != ShellType.Default)
			{
				try
				{
					_hookID = InterceptKeys.SetHook(_proc);
				}
				catch
				{
					if (_hookID != IntPtr.Zero)
						InterceptKeys.UnhookWindowsHookEx(_hookID);
				}
			}
		}

		protected virtual Bootstrapper CreateBootstrapper()
		{
			return new Bootstrapper();
		}
		protected override void OnStartup(StartupEventArgs e)
        {
            var culture = new CultureInfo(ConfigurationManager.AppSettings["DefaultCulture"]);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            base.OnStartup(e);
			try
			{
				if (CheckIntegrateCommandLineArguments(e.Args))
				{
					Shutdown();
					return;
				}

				ApplicationService.Closing += ApplicationService_Closing;
				ApplicationService.Closed += ApplicationService_Closed;
				ThemeHelper.LoadThemeFromRegister();
#if DEBUG
				bool trace = false;
				BindingErrorListener.Listen(m => { if (trace) MessageBox.Show(m); });
#endif
				_bootstrapper = CreateBootstrapper();
				_bootstrapper.InitializeCommandLineArguments(e.Args);
				bool result;
				//using (new DoubleLaunchLocker(SignalId, WaitId, true))
					result = _bootstrapper.Initialize();
				if (!result)
				{
					ApplicationService.ShutDown();
					return;
				}
				AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

				if (GlobalSettingsHelper.GlobalSettings.RunRevisor)
					StartRevisor();
			}
			catch (StartupCancellationException)
			{
				ApplicationService.ShutDown();
			}
			catch (Exception ex)
			{
				Logger.Error(ex, "App.OnStartup");
				MessageBoxService.ShowError(CommonErrors.Loading_Exception);
			}
			finally
			{
				ServiceFactory.StartupService.Close();
			}
		}

		void StartRevisor()
		{
			try
			{
				var path = System.Reflection.Assembly.GetExecutingAssembly();
				RegistrySettingsHelper.SetString("FireMonitorPath", path.Location);
				RegistrySettingsHelper.SetBool("FireMonitor.IsRunning", true);
				RegistrySettingsHelper.SetBool("IsException", false);
				var isAutoConnect = RegistrySettingsHelper.GetBool("isAutoConnect");
				if (isAutoConnect)
				{
					RegistrySettingsHelper.SetBool("isAutoConnect", false);
				}
				RevisorLoadHelper.Load();
			}
			catch (Exception e)
			{
				Logger.Error(e, "App.StartRevisor");
			}
		}

		void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			IsClosingOnException = true;
			Logger.Error(e.ExceptionObject as Exception, "App.CurrentDomain_UnhandledException");
			_bootstrapper.RestartApplication();
			Environment.Exit(0);
		}

		private void ApplicationService_Closing(object sender, CancelEventArgs e)
		{
			if (e.Cancel)
				return;

			if (ApplicationService.Modules != null)
				foreach (var module in ApplicationService.Modules)
					module.Dispose();
			AlarmPlayerHelper.Dispose();
			ClientSettings.SaveSettings();
			FiresecManager.Disconnect();
			if (ShellIntegrationHelper.IsIntegrated && !IsClosingOnException)
				ShellIntegrationHelper.Reboot();
			RegistrySettingsHelper.SetBool("FireMonitor.IsRunning", false);
		}

		private void ApplicationService_Closed(object sender, EventArgs e)
		{
			Current.Shutdown();
		}

		private bool CheckIntegrateCommandLineArguments(string[] args)
		{
			if (args == null || args.Count() != 1) return false;

			switch (args[0])
			{
				case "/integrate":
					ShellIntegrationHelper.Integrate();
					MessageBox.Show(CommonResources.OTIntegrated);
					return true;

				case "/deintegrate":
					ShellIntegrationHelper.Desintegrate();
                    MessageBox.Show(CommonResources.OTDisintegrated);
					return true;
			}
			return false;
		}

		public static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
		{
			if (nCode >= 0)
			{
				bool Alt = (WinForms.Control.ModifierKeys & Keys.Alt) != 0;
				bool Control = (WinForms.Control.ModifierKeys & Keys.Control) != 0;

				//Prevent ALT-TAB and CTRL-ESC by eating TAB and ESC. Also kill Windows Keys.
				int vkCode = Marshal.ReadInt32(lParam);
				Keys key = (Keys)vkCode;

				//if (Alt && key == Keys.F4)
				//{
				//	Application.Current.Shutdown();
				//	return (IntPtr)1; //handled
				//}
				if (key == Keys.LWin || key == Keys.RWin) return (IntPtr)1; //handled
				if (Alt && key == Keys.Tab) return (IntPtr)1; //handled
				if (Alt && key == Keys.Space) return (IntPtr)1; //handled
				if (Control && key == Keys.Escape) return (IntPtr)1;
				//if (key == Keys.None) return (IntPtr)1; //handled
				//if (key <= Keys.Back) return (IntPtr)1; //handled
				//if (key == Keys.Menu) return (IntPtr)1; //handled
				//if (key == Keys.Pause) return (IntPtr)1; //handled
				//if (key == Keys.Help) return (IntPtr)1; //handled
				//if (key == Keys.Sleep) return (IntPtr)1; //handled
				//if (key == Keys.Apps) return (IntPtr)1; //handled
				//if (key >= Keys.KanaMode && key <= Keys.HanjaMode) return (IntPtr)1; //handled
				//if (key >= Keys.IMEConvert && key <= Keys.IMEModeChange) return (IntPtr)1; //handled
				//if (key >= Keys.BrowserBack && key <= Keys.BrowserHome) return (IntPtr)1; //handled
				//if (key >= Keys.MediaNextTrack && key <= Keys.OemClear) return (IntPtr)1; //handled

				Debug.WriteLine(vkCode.ToString() + " " + key);


			}
			return InterceptKeys.CallNextHookEx(_hookID, nCode, wParam, lParam);
		}

		[STAThread]
		private static void Main()
		{
			ServiceFactory.StartupService.Run();
			var app = new App();
			app.InitializeComponent();
			app.Run();
		}
	}
}