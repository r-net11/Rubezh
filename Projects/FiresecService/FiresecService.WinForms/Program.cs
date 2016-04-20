using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using FiresecService.Views;
using FiresecService.Presenters;
using Common;
using Infrastructure.Automation;
using Infrastructure.Common.Windows;

namespace FiresecService
{
	static class Program
	{
		public static ApplicationContext AppContext { get; set; }
		public static NotifyIcon SystemTray { get; set; }

		[STAThread]
		static void Main()
		{
			ServerLoadHelper.SetLocation(System.Reflection.Assembly.GetExecutingAssembly().Location);
			ServerLoadHelper.SetStatus(FSServerState.Opening);

			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			Application.ApplicationExit += Application_ApplicationExit;
			AppContext = new ApplicationContext();

			var _contextMenuNotifyIcon = new ContextMenuStrip();
			var menuItem = new ToolStripMenuItem();
			menuItem.Name = "_ToolStripMenuItemShowWindow";
			menuItem.Text = "Показать";
			menuItem.Click += EventHandler_MenuItemShowWindow_Click;
			_contextMenuNotifyIcon.Items.Add(menuItem);
			menuItem = new ToolStripMenuItem();
			menuItem.Name = "_ToolStripMenuItemExit";
			menuItem.Text = "Выход";
			menuItem.Click += EventHandler_MenuItemExit_Click;
			_contextMenuNotifyIcon.Items.Add(menuItem);

			SystemTray = new NotifyIcon();
			SystemTray.Icon = Properties.Resources.Firesec;
			SystemTray.ContextMenuStrip = _contextMenuNotifyIcon;
			SystemTray.Visible = true;
			SystemTray.Text = "Сервер приложений Глобал";

			var view = new MainView();
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
				//BalloonHelper.ShowFromServer("Ошибка во время загрузки");
				ShowBalloonTip(5000, "Ошибка", "Ошибка во время загрузки", ToolTipIcon.Error);
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
			//BalloonHelper.ShowFromServer("Перезагрузка");
			ShowBalloonTip(5000, "Внимание", "Перезагрузка", ToolTipIcon.Info);
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

		/// <summary>
		/// Tray: Выводит сообщение
		/// </summary>
		/// <param name="timeOut">Время отображения сообщения, мсек</param>
		/// <param name="title">Заголовок сообщения</param>
		/// <param name="text">Текс сообщения</param>
		/// <param name="icon">Иконка сообщения</param>
		public static void ShowBalloonTip(int timeOut, string title, string text, ToolTipIcon icon)
		{
			SystemTray.ShowBalloonTip(timeOut, title, text, icon);
		}
	}
}