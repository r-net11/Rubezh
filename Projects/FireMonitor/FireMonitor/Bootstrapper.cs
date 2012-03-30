using System.Configuration;
using System.Linq;
using System.Windows;
using AlarmModule;
using AlarmModule.Events;
using Common;
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
        bool showOnlyVideo;

        void InitializeVideoSettings()
        {
            showOnlyVideo = ConfigurationHelper.ShowOnlyVideo;
            VideoService.Initialize(ConfigurationHelper.LibVlcDllsPath);
        }

        public void Initialize()
        {
            InitializeVideoSettings();

            RegisterServices();
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));

            var preLoadWindow = new PreLoadWindow();
            
            var connectResult = false;
            if (ConfigurationHelper.AutoConnect)
                connectResult = LoginViewModel.DefaultConnect();
            else
            {
                var loginViewModel = new LoginViewModel(LoginViewModel.PasswordViewType.Connect);
                connectResult = ServiceFactory.UserDialogs.ShowModalWindow(loginViewModel);
            }

            if (connectResult)
            {
                preLoadWindow.PreLoadText = "Инициализация компонент...";
                preLoadWindow.Show();
                FiresecManager.SelectiveFetch();

                if (FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Oper_Login))
                {
                    ClientSettings.LoadSettings();

                    var shellView = new ShellView();
                    ServiceFactory.ShellView = shellView;

                    if (showOnlyVideo)
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
            FiresecManager.SelectiveFetch(false);

            DevicesModule.DevicesModuleLoader.CreateViewModels();
            JournalModule.JournalModuleLoader.CreateViewModels();
            AlarmModule.AlarmModuleLoader.CreateViewModels();
            ReportsModule.ReportsModuleLoader.CreateViewModels();
            CallModule.CallModuleLoader.CreateViewModels();
            PlansModule.PlansModuleLoader.CreateViewModels();
        }
    }
}