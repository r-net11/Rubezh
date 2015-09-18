using System;
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
		static Thread WindowThread = null;
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
					MainViewModel = new MainViewModel();
					ApplicationService.Run(MainViewModel, false, false);
				}
				catch (Exception e)
				{
					Logger.Error(e, "Исключение при вызове Bootstrapper.OnWorkThread");

					BalloonHelper.ShowFromServer("Ошибка во время загрузки");
				}
				System.Windows.Threading.Dispatcher.Run();
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове Bootstrapper.Run");
				Close();
			}
		}

		public static void Close()
		{
			ServerLoadHelper.SetStatus(FSServerState.Closed);
			if (WindowThread != null)
			{
				WindowThread.Interrupt();
				WindowThread = null;
			}
			System.Environment.Exit(1);

			Process.GetCurrentProcess().Kill();
		}
	}
}