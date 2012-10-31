using System;
using System.IO;
using System.Threading;
using Common;
using FiresecService.Configuration;
using FiresecService.Service;
using FiresecService.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Theme;
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
                AppSettingsHelper.InitializeAppSettings();
                ThemeHelper.LoadThemeFromRegister();
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

                UILogger.Log("Открытие хоста");
                FiresecServiceManager.Open();
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
    }
}