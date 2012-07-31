﻿using System;
using System.IO;
using System.Threading;
using System.Windows;
using Common;
using FiresecService.Service;
using FiresecService.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using FiresecService.OPC;

namespace FiresecService
{
	public static class Bootstrapper
	{
		static Thread WindowThread = null;
		static MainViewModel MainViewModel;
		static AutoResetEvent MainViewStartedEvent = new AutoResetEvent(false);

		public static void Run()
		{
			try
			{
				InitializeAppSettings();
				var directoryInfo = new DirectoryInfo(Environment.GetCommandLineArgs()[0]);
				Environment.CurrentDirectory = directoryInfo.FullName.Replace(directoryInfo.Name, "");

				var resourceService = new ResourceService();
				resourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "DataTemplates/Dictionary.xaml"));
				resourceService.AddResource(new ResourceDescription(typeof(ApplicationService).Assembly, "Windows/DataTemplates/Dictionary.xaml"));

				WindowThread = new Thread(new ThreadStart(OnWorkThread));
				WindowThread.Priority = ThreadPriority.Highest;
				WindowThread.SetApartmentState(ApartmentState.STA);
				WindowThread.IsBackground = true;
				WindowThread.Start();
				MainViewStartedEvent.WaitOne();

				ClientsCash.InitializeComServers();
				UILogger.Log("Открытие хоста");
				var isHostOpened = FiresecServiceManager.Open();
				if (AppSettings.RunOPC)
				{
					UILogger.Log("Запуск OPC сервера");
					FiresecOPCManager.Start();
				}
				UILogger.Log("Готово");
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове Bootstrapper.Run");
				UILogger.Log("Ошибка при запуске сервера", true);
				Close();
			}
		}

		static void OnWorkThread()
		{
			try
			{
				MainViewModel = new MainViewModel();
				ApplicationService.Run(MainViewModel);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове Bootstrapper.OnWorkThread");
			}
			MainViewStartedEvent.Set();
			System.Windows.Threading.Dispatcher.Run();
		}

		public static void Close()
		{
			if (WindowThread != null)
			{
				WindowThread.Interrupt();
				WindowThread = null;
			}

			System.Environment.Exit(1);
		}

		static void InitializeAppSettings()
		{
			AppSettings.OldFiresecLogin = System.Configuration.ConfigurationManager.AppSettings["OldFiresecLogin"] as string;
			AppSettings.OldFiresecPassword = System.Configuration.ConfigurationManager.AppSettings["OldFiresecPassword"] as string;
			AppSettings.ServiceAddress = System.Configuration.ConfigurationManager.AppSettings["ServiceAddress"] as string;
			AppSettings.LocalServiceAddress = System.Configuration.ConfigurationManager.AppSettings["LocalServiceAddress"] as string;
			AppSettings.OverrideFiresec1Config = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["OverrideFiresec1Config"] as string);
			AppSettings.IsImitatorVisible = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsImitatorVisible"] as string);
			AppSettings.RunOPC = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["RunOPC"] as string);
#if DEBUG
			AppSettings.IsDebug = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsDebug"] as string);
#endif
		}
	}
}