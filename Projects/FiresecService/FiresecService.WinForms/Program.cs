using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using FiresecService.Views;
using FiresecService.Presenters;
using Common;
using Infrastructure.Automation;
using Infrastructure.Common;

namespace FiresecService
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			ServerLoadHelper.SetLocation(System.Reflection.Assembly.GetExecutingAssembly().Location);
			ServerLoadHelper.SetStatus(FSServerState.Opening);

			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.ApplicationExit += Application_ApplicationExit;
			var view = new MainWinFormView();
			var presenter = new MainPresenter(view);
			view.WindowState = FormWindowState.Minimized;

			try
			{
				Bootstrapper.Run();
			}
			catch (Exception ex)
			{
				Logger.Error(ex, "App.OnStartup");
				//BalloonHelper.ShowFromServer("Ошибка во время загрузки");
				MessageBox.Show("Ошибка во время загрузки", "Ошибка", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			Application.Run(view);
		}

		static void Application_ApplicationExit(object sender, EventArgs e)
		{
			AutomationProcessor.Terminate();
			Bootstrapper.Close();
		}

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Logger.Error((Exception)e.ExceptionObject, "App.CurrentDomain_UnhandledException");
			//BalloonHelper.ShowFromServer("Перезагрузка");
			var processStartInfo = new ProcessStartInfo()
			{
				//FileName = Application.ResourceAssembly.Location
				FileName = (AppDomain.CurrentDomain.GetAssemblies())[0].Location
			};
			Process.Start(processStartInfo);
			Bootstrapper.Close();
			//Application.Current.MainWindow.Close();
			//Application.Current.Shutdown();
			Application.Exit();
		}
	}
}