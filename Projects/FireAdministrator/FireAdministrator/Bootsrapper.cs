using System.Configuration;
using System.Linq;
using System.Windows;
using Common;
using Controls.MessageBox;
using FireAdministrator.ViewModels;
using FireAdministrator.Views;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;

namespace FireAdministrator
{
    public class Bootsrapper
    {
        public void Initialize()
        {
            RegisterServices();
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));

            var preLoadWindow = new PreLoadWindow();

            var connectResult = false;
            if (ConfigurationHelper.AutoConnect)
                connectResult = LoginViewModel.DefaultConnect();
            else
            {
                var loginViewModel = new LoginViewModel();
                connectResult = ServiceFactory.UserDialogs.ShowModalWindow(loginViewModel);
            }

            if (connectResult)
            {
                preLoadWindow.PreLoadText = "Инициализация компонент...";
                preLoadWindow.Show();
                
                FiresecManager.SelectiveFetch();

                if (FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Adm_ViewConfig) == false)
                {
                    MessageBoxService.Show("Нет прав на работу с программой");
                    FiresecManager.Disconnect();
                }
                else
                {
                    InitializeKnownModules();

                    var ShellView = new ShellView();
                    ServiceFactory.ShellView = ShellView;
                    Application.Current.MainWindow = ShellView;
                    Application.Current.MainWindow.Show();
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
            ServiceFactory.Initialize(new LayoutService(), new UserDialogService(), new ProgressService());
            ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Subscribe(x => { InitializeKnownModules(); });
        }

        static void InitializeKnownModules()
        {
            var devicesModule = new DevicesModule.DevicesModule();
            var libraryModule = new LibraryModule.LibraryModule();
            var plansModule = new PlansModule.PlansModule();
            var securityModule = new SecurityModule.SecurityModule();
            var filtersModule = new FiltersModule.FilterModule();
            var soundsModule = new SoundsModule.SoundsModule();
            var instructionsModule = new InstructionsModule.InstructionsModule();
            var settingsModule = new SettingsModule.SettingsModule();
            var groupControllerViewModel = new GroupControllerModule.GroupControllerModule();
            var videoViewModel = new VideoModule.VideoModule();

            ServiceFactory.SaveService.Reset();

            string libVlcDllsPath = ConfigurationManager.AppSettings["LibVlcDllsPath"] as string;
            VideoService.Initialize(libVlcDllsPath);
        }
    }
}