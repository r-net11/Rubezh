﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Common;
using FiresecService.Service;
using FiresecService.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.BalloonTrayTip;
using Infrastructure.Common.Windows;

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
				Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
				PatchManager.Patch();
                FiresecDB.DatabaseHelper.ConnectionString = @"Data Source=" + AppDataFolderHelper.GetDBFile("Firesec.sdf") + ";Password=adm;Max Database Size=4000";
                Logger.Trace(SystemInfo.GetString());
                AppSettingsHelper.InitializeAppSettings();
                var resourceService = new ResourceService();
                resourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "DataTemplates/Dictionary.xaml"));
                resourceService.AddResource(new ResourceDescription(typeof(ApplicationService).Assembly, "Windows/DataTemplates/Dictionary.xaml"));
                WindowThread = new Thread(new ThreadStart(OnWorkThread));
                WindowThread.Priority = ThreadPriority.Highest;
                WindowThread.SetApartmentState(ApartmentState.STA);
                WindowThread.IsBackground = true;
                WindowThread.Start();
                MainViewStartedEvent.WaitOne();

                UILogger.Log("Открытие хоста");
                FiresecServiceManager.Open();
                UILogger.Log("Готово");
                ServerLoadHelper.SetStatus(FSServerState.Opened);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Исключение при вызове Bootstrapper.Run");
                UILogger.Log("Ошибка при запуске сервера");
                BalloonHelper.Show("Сервер приложений Firesec", "Ошибка во время загрузки");
                Close();
            }
        }

        private static void OnWorkThread()
        {
            try
            {
                MainViewModel = new MainViewModel();
                ApplicationService.Run(MainViewModel, false, false);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Исключение при вызове Bootstrapper.OnWorkThread");
                BalloonHelper.Show("Сервер приложений Firesec", "Ошибка во время загрузки");
            }
            MainViewStartedEvent.Set();
            System.Windows.Threading.Dispatcher.Run();
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

#if DEBUG
			return;
#endif
            Process.GetCurrentProcess().Kill();
        }
    }
}