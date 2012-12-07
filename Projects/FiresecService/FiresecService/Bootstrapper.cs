using System;
using System.IO;
using System.Threading;
using Common;
using FiresecService.Service;
using FiresecService.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.BalloonTrayTip.ViewModels;
using Infrastructure.Common.BalloonTrayTip;
using System.Reflection;

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
				Logger.Trace(SystemInfo.GetString());
				AppSettingsHelper.InitializeAppSettings();
				Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

                var resourceService = new ResourceService();
                resourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "DataTemplates/Dictionary.xaml"));
                resourceService.AddResource(new ResourceDescription(typeof(ApplicationService).Assembly, "Windows/DataTemplates/Dictionary.xaml"));
                resourceService.AddResource(new ResourceDescription(typeof(BalloonToolTipViewModel).Assembly, "BalloonTrayTip/DataTemplates/Dictionary.xaml"));

                BalloonHelper.Initialize();
                
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
                BalloonHelper.ShowWarning("Сервер приложений Firesec", "Ошибка во время загрузки");
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
                BalloonHelper.ShowWarning("Сервер приложений Firesec", "Ошибка во время загрузки");
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
        }
    }
}