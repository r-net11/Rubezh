using Common;
using Infrastructure.Common.Windows.BalloonTrayTip;
using Infrastructure.Common.Windows.Services;
using Infrastructure.Common.Windows.Theme;
using Infrastructure.Common.Windows.Windows;
using Microsoft.Win32;
using Resurs.ViewModels;
using Resurs.Views;
using ResursAPI;
using ResursAPI.License;
using ResursDAL;
using ResursRunner;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Resurs
{
	public static class Bootstrapper
	{
		public static MainViewModel MainViewModel;
		public static MainView MainView;

		public static void Run(bool showWindow)
		{
			try
			{
				AddToAutorun();
				ThemeHelper.LoadThemeFromRegister();
				Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
				ServiceFactoryBase.ResourceService.AddResource(typeof(ApplicationService).Assembly, "Windows/DataTemplates/Dictionary.xaml");
				ServiceFactoryBase.ResourceService.AddResource(typeof(Bootstrapper).Assembly, "Main/DataTemplates/Dictionary.xaml");
				ServiceFactoryBase.ResourceService.AddResource(typeof(Bootstrapper).Assembly, "Devices/DataTemplates/Dictionary.xaml");
				ServiceFactoryBase.ResourceService.AddResource(typeof(Bootstrapper).Assembly, "Consumers/DataTemplates/Dictionary.xaml");
				ServiceFactoryBase.ResourceService.AddResource(typeof(Bootstrapper).Assembly, "Reports/DataTemplates/Dictionary.xaml");
				ServiceFactoryBase.ResourceService.AddResource(typeof(Bootstrapper).Assembly, "Users/DataTemplates/Dictionary.xaml");
				ServiceFactoryBase.ResourceService.AddResource(typeof(Bootstrapper).Assembly, "JournalEvents/DataTemplates/Dictionary.xaml");
				ServiceFactoryBase.ResourceService.AddResource(typeof(Bootstrapper).Assembly, "Tariffs/DataTemplates/Dictionary.xaml");
				ServiceFactoryBase.ResourceService.AddResource(typeof(Bootstrapper).Assembly, "Deposits/DataTemplates/Dictionary.xaml");

				LicenseManager.CurrentLicenseInfo = LicenseManager.TryLoad(FolderHelper.GetFile("Resurs.license"));
				if (LicenseManager.CurrentLicenseInfo == null)
					BalloonHelper.Show("АРМ Ресурс", "Ошибка лицензии");
#if DEBUG
				LicenseManager.CurrentLicenseInfo = new ResursLicenseInfo { DevicesCount = 10 }; //TODO: убрать
#endif

				try
				{
					App.Current.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
					Activate(showWindow);
				}
				catch (Exception e)
				{
					Logger.Error(e, "Исключение при вызове Bootstrapper.OnWorkThread");
					BalloonHelper.Show("АРМ Ресурс", "Ошибка во время загрузки");
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове Bootstrapper.Run");
				Close();
			}
		}

		public static void Activate()
		{
			Activate(true);
		}
		public static void Activate(bool showWindow)
		{
			if (showWindow && DbCache.CurrentUser == null)
			{
				var startupViewModel = new StartupViewModel();
				if (!DialogService.ShowModalWindow(startupViewModel))
					return;
			}
			
			if (MainViewModel == null)
				MainViewModel = new MainViewModel();
			
			if (MainView == null)
			{
				MainView = new MainView();
				MainView.DataContext = MainViewModel;
			}

			if (showWindow)
			{
				MainViewModel.UpdateTabsIsVisible();
				MainView.WindowState = System.Windows.WindowState.Normal;
				MainView.Show();
				MainView.Activate();
				MainView.ShowInTaskbar = true;
			}
		}

		static void AddToAutorun()
		{
			try
			{
				using (var registryKey = Registry.CurrentUser.CreateSubKey(@"software\Microsoft\Windows\CurrentVersion\Run"))
				{
					if (registryKey != null)
						registryKey.SetValue("Resurs", string.Format("\"{0}\" -hide", Application.ExecutablePath));
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при попытке добавления программы в автозагрузку");
			}
			
		}

		public static void Close()
		{
			System.Environment.Exit(1);
			Process.GetCurrentProcess().Kill();
		}
	}
}