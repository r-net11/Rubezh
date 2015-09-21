﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Common;
using Resurs.Service;
using Resurs.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.BalloonTrayTip;
using Infrastructure.Common.Windows;
using FiresecAPI;

namespace Resurs
{
	public static class Bootstrapper
	{
		static MainViewModel MainViewModel;

		public static void Run()
		{
			try
			{
				Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
				Logger.Trace(SystemInfo.GetString());
				var resourceService = new ResourceService();
				resourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "DataTemplates/Dictionary.xaml"));
				resourceService.AddResource(new ResourceDescription(typeof(ApplicationService).Assembly, "Windows/DataTemplates/Dictionary.xaml"));

				try
				{
					var mainView = new Resurs.Views.MainView();
					var mainViewModel = new MainViewModel();
					mainView.DataContext = mainViewModel;
					mainView.Show();
				}
				catch (Exception e)
				{
					Logger.Error(e, "Исключение при вызове Bootstrapper.OnWorkThread");

					BalloonHelper.ShowFromServer("Ошибка во время загрузки");
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове Bootstrapper.Run");
				Close();
			}
		}

		public static void Close()
		{
			System.Environment.Exit(1);
			Process.GetCurrentProcess().Kill();
		}
	}
}