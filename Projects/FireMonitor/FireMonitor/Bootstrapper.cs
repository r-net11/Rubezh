using System;
using System.Configuration;
using System.Linq;
using System.Windows;
using AlarmModule;
using AlarmModule.Events;
using Controls.MessageBox;
using FireMonitor.ViewModels;
using FireMonitor.Views;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Common;

namespace FireMonitor
{
    public class Bootstrapper
    {
        public void Initialize()
        {
            AppConfigHelper.InitializeAppSettings();
            if (!SingleLaunchHelper.Check("FireMonitor"))
            {
                Application.Current.Shutdown();
            }

            VideoService.Initialize(ServiceFactory.AppSettings.LibVlcDllsPath);

            RegisterServices();
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));

            var preLoadWindow = new PreLoadWindow();
            
            var connectResult = false;
            var loginViewModel = new LoginViewModel(LoginViewModel.PasswordViewType.Connect);
            if (ServiceFactory.AppSettings.AutoConnect)
                connectResult = loginViewModel.AutoConnect();
            if (connectResult == false)
            {
                connectResult = ServiceFactory.UserDialogs.ShowModalWindow(loginViewModel);
            }

            if (connectResult)
            {
                preLoadWindow.PreLoadText = "Инициализация компонент...";
                preLoadWindow.Show();
                FiresecManager.GetConfiguration();
                FiresecManager.GetStates();

                var operationResult = FiresecManager.FiresecService.CheckHaspPresence();
                if (operationResult.HasError)
                {
                    MessageBoxService.ShowWarning("HASP-ключ на сервере не обнаружен. Время работы приложения будет ограничено");
                }
                var serverStatus = FiresecManager.FiresecService.GetStatus();
                if (serverStatus != null)
                {
                    MessageBoxService.ShowWarning(serverStatus);
                }

                if (FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Oper_Login))
                {
                    ClientSettings.LoadSettings();

                    var shellView = new ShellView();
                    ServiceFactory.ShellView = shellView;

                    if (ServiceFactory.AppSettings.ShowOnlyVideo)
                    {
                        var alarmVideoWather = new AlarmVideoWather();
                        preLoadWindow.Close();
                        return;
                    }

                    InitializeModules();

                    App.Current.MainWindow = shellView;
                    App.Current.MainWindow.Show();

                    ServiceFactory.Events.GetEvent<ShowAlarmsEvent>().Publish(null);

                    FiresecCallbackService.ConfigurationChangedEvent += new Action(OnConfigurationChanged);

                    ReportsModule.ReportsModuleLoader.PreLoad();
                }
                else
                {
                    MessageBoxService.Show("Нет прав на работу с программой");
                    FiresecManager.Disconnect();
                }

                preLoadWindow.Close();
                SingleLaunchHelper.KeepAlive();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        static void RegisterServices()
        {
            ServiceFactory.Initialize(new LayoutService(), new UserDialogService(), new SecurityService());
        }

        void InitializeModules()
        {
            DevicesModuleLoader = new DevicesModule.DevicesModuleLoader();
            JournalModuleLoader = new JournalModule.JournalModuleLoader();
            AlarmModuleLoader = new AlarmModule.AlarmModuleLoader();
            ReportsModuleLoader = new ReportsModule.ReportsModuleLoader();
            CallModuleLoader = new CallModule.CallModuleLoader();
            PlansModuleLoader = new PlansModule.PlansModuleLoader();
            if (ServiceFactory.AppSettings.ShowGK)
            {
                GKModuleLoader = new GKModule.GKModuleLoader();
            }
        }

        DevicesModule.DevicesModuleLoader DevicesModuleLoader;
        JournalModule.JournalModuleLoader JournalModuleLoader;
        AlarmModule.AlarmModuleLoader AlarmModuleLoader;
        ReportsModule.ReportsModuleLoader ReportsModuleLoader;
        CallModule.CallModuleLoader CallModuleLoader;
        PlansModule.PlansModuleLoader PlansModuleLoader;
        GKModule.GKModuleLoader GKModuleLoader;

        void OnConfigurationChanged()
        {
            ServiceFactory.Layout.Close();
            FiresecManager.GetConfiguration(false);
            FiresecManager.GetStates();

            ServiceFactory.ShellView.Dispatcher.Invoke(new Action(() => { OnInitializeViewModels(); }));
        }

        void OnInitializeViewModels()
        {
            DevicesModule.DevicesModuleLoader.Initialize();
            JournalModule.JournalModuleLoader.Initialize();
            AlarmModule.AlarmModuleLoader.Initialize();
            ReportsModule.ReportsModuleLoader.Initialize();
            CallModule.CallModuleLoader.Initialize();
            PlansModule.PlansModuleLoader.Initialize();
            if (ServiceFactory.AppSettings.ShowGK)
            {
                GKModuleLoader = new GKModule.GKModuleLoader();
            }

            ServiceFactory.Events.GetEvent<ShowAlarmsEvent>().Publish(null);
        }
    }
}