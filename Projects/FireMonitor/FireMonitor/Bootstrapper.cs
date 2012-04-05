using System;
using System.Configuration;
using System.Linq;
using System.Windows;
using AlarmModule;
using AlarmModule.Events;
using Controls.MessageBox;
using Controls.MessageBox;
using FireMonitor.ViewModels;
using FireMonitor.Views;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace FireMonitor
{
    public class Bootstrapper
    {
        public void Initialize()
        {
            InitializeAppSettings();
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

                    FiresecCallbackService.ConfigurationChangedEvent += new System.Action(OnConfigurationChanged);
                }
                else
                {
                    MessageBoxService.Show("Нет прав на работу с программой");
                    FiresecManager.Disconnect();
                }

                preLoadWindow.Close();
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
        }

        DevicesModule.DevicesModuleLoader DevicesModuleLoader;
        JournalModule.JournalModuleLoader JournalModuleLoader;
        AlarmModule.AlarmModuleLoader AlarmModuleLoader;
        ReportsModule.ReportsModuleLoader ReportsModuleLoader;
        CallModule.CallModuleLoader CallModuleLoader;
        PlansModule.PlansModuleLoader PlansModuleLoader;

        void OnConfigurationChanged()
        {
            ServiceFactory.Layout.Close();
            FiresecManager.GetConfiguration(false);

            DevicesModule.DevicesModuleLoader.CreateViewModels();
            JournalModule.JournalModuleLoader.CreateViewModels();
            AlarmModule.AlarmModuleLoader.CreateViewModels();
            ReportsModule.ReportsModuleLoader.CreateViewModels();
            CallModule.CallModuleLoader.CreateViewModels();
            PlansModule.PlansModuleLoader.CreateViewModels();
        }

        static void InitializeAppSettings()
        {
            var appSettings = new AppSettings();
            appSettings.ServiceAddress = ConfigurationManager.AppSettings["ServiceAddress"] as string;
            appSettings.DefaultLogin = ConfigurationManager.AppSettings["DefaultLogin"] as string;
            appSettings.DefaultPassword = ConfigurationManager.AppSettings["DefaultPassword"] as string;
            appSettings.AutoConnect = Convert.ToBoolean(ConfigurationManager.AppSettings["AutoConnect"] as string);
            appSettings.LibVlcDllsPath = ConfigurationManager.AppSettings["LibVlcDllsPath"] as string;
            appSettings.ShowOnlyVideo = Convert.ToBoolean(ConfigurationManager.AppSettings["ShowOnlyVideo"] as string);
            appSettings.IsDebug = Convert.ToBoolean(ConfigurationManager.AppSettings["IsDebug"] as string);
            ServiceFactory.AppSettings = appSettings;
        }
    }
}