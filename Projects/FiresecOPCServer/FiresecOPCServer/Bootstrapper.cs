using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecClient;
using FiresecAPI.Models;
using System.Configuration;
using Infrastructure.Common.Theme;
using Infrastructure.Common.Windows;
using FiresecOPCServer.ViewModels;
using Infrastructure.Common;
using System.Threading;
using Common;
using Firesec;

namespace FiresecOPCServer
{
    public static class Bootstrapper
    {
        static Thread WindowThread = null;
        static MainViewModel MainViewModel;
        static AutoResetEvent MainViewStartedEvent = new AutoResetEvent(false);

        public static void Run()
        {
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

            UILogger.Log("Соединение с сервером");
            for (int i = 1; i <= 10; i++)
            {
                var message = FiresecManager.Connect(ClientType.OPC, AppSettingsManager.ServerAddress, AppSettingsManager.Login, AppSettingsManager.Password);
                if (message == null)
                    break;
                Thread.Sleep(5000);
                if (i == 10)
                {
                    UILogger.Log("Ошибка соединения с сервером: " + message);
                    return;
                }
            }

            InitializeFs();
            //UILogger.Log("Старт полинга сервера");
            //FiresecManager.StartPoll(false);
            FiresecOPCManager.Start();
            //SafeFiresecService.ConfigurationChangedEvent += new Action(OnConfigurationChangedEvent);
            UILogger.Log("Готово");
        }

        static void InitializeFs()
        {
            UILogger.Log("Остановка Socket Server");
            UILogger.Log("Загрузка конфигурации с сервера");
            FiresecManager.GetConfiguration();
            UILogger.Log("Загрузка драйвера устройств");
            FiresecManager.InitializeFiresecDriver(true);
            UILogger.Log("Синхронизация конфигурации");
            FiresecManager.FiresecDriver.Synchronyze();
            UILogger.Log("Старт мониторинга");
            FiresecManager.FiresecDriver.StartWatcher(true, false);
			FiresecManager.FSAgent.Start();
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
            FiresecManager.Disconnect();
            if (WindowThread != null)
            {
                WindowThread.Interrupt();
                WindowThread = null;
            }

            System.Environment.Exit(1);
        }

        static void OnConfigurationChangedEvent()
        {
            UILogger.Log("Перезагрузка конфигурации");
            FiresecManager.GetConfiguration();
            UILogger.Log("Синхронизация конфигурации");
            FiresecManager.FiresecDriver.Synchronyze();
            UILogger.Log("Перезапуск OPC Сервера");
            FiresecOPCManager.OPCRefresh();
        }
    }
}