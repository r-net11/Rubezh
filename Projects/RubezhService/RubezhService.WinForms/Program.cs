using Common;
using RubezhService.Presenters;
using RubezhService.Views;
using Infrastructure.Automation;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace RubezhService
{
	static class Program
	{
		public static ApplicationContext AppContext { get; set; }
		//public static NotifyIcon SystemTray { get; set; }

		[STAThread]
		static void Main()
		{

			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			Application.ApplicationExit += Application_ApplicationExit;
			AppContext = new ApplicationContext();

			var view = new MainView { Visible = true };
			var presenter = new MainPresenter(view);

			AppContext.MainForm = null;
			AppContext.MainForm = view;

			try
			{
				Bootstrapper.Run();
			}
			catch (Exception ex)
			{
				Logger.Error(ex, "App.OnStartup");
				return;
			}

			Application.Run(AppContext);
		}

		static void Application_ApplicationExit(object sender, EventArgs e)
		{
			AutomationProcessor.Terminate();
			Bootstrapper.Close();
		}

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Logger.Error((Exception)e.ExceptionObject, "App.CurrentDomain_UnhandledException");
			var processStartInfo = new ProcessStartInfo()
			{
				//FileName = Application.ResourceAssembly.Location
				FileName = (AppDomain.CurrentDomain.GetAssemblies())[0].Location
			};
			Process.Start(processStartInfo);
			Bootstrapper.Close();
			Application.Exit();
		}

		static void EventHandler_MenuItemShowWindow_Click(object sender, EventArgs e)
		{
			if (AppContext.MainForm.Visible == false)
			{
				AppContext.MainForm.StartPosition = FormStartPosition.CenterScreen;
				AppContext.MainForm.WindowState = FormWindowState.Normal;
				AppContext.MainForm.ShowInTaskbar = true;
				AppContext.MainForm.Visible = true;
			}
		}

		static void EventHandler_MenuItemExit_Click(object sender, EventArgs e)
		{
			var result = MessageBox.Show("Вы уверены, что хотите остановить сервер?", "Глобал",
				MessageBoxButtons.YesNo, MessageBoxIcon.Question);

			if (result == DialogResult.Yes)
			{
				Application.Exit();
			}
		}
	}
}