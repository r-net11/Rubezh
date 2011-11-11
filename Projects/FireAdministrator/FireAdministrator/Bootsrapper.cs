using System.Linq;
using FireAdministrator.ViewModels;
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
            var loginViewModel = new LoginViewModel();
            if (ServiceFactory.UserDialogs.ShowModalWindow(loginViewModel))
            {
                preLoadWindow.Show();

                FiresecManager.SelectiveFetch();

                if (FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Adm_ViewConfig) == false)
                {
                    DialogBox.DialogBox.Show("Нет прав на работу с программой");
                    FiresecManager.Disconnect();
                }
                else
                {
                    InitializeKnownModules();

                    var ShellView = new ShellView();
                    ServiceFactory.ShellView = ShellView;
                    App.Current.MainWindow = ShellView;
                    App.Current.MainWindow.Show();
                }
                preLoadWindow.Close();
            }
            else
            {
                App.Current.Shutdown();
            }
        }

        static void RegisterServices()
        {
            ServiceFactory.Initialize(new LayoutService(), new UserDialogService(), new ResourceService());
            ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Subscribe(x => { InitializeKnownModules(); });
        }

        static void InitializeKnownModules()
        {
            var devicesModule = new DevicesModule.DevicesModule();
            devicesModule.Initialize();

            var libraryModule = new LibraryModule.LibraryModule();
            libraryModule.Initialize();

            var plansModule = new PlansModule.PlansModule();
            plansModule.Initialize();

            var sequrityModule = new SecurityModule.SecurityModule();
            sequrityModule.Initialize();

            var filtersModule = new FiltersModule.FilterModule();
            filtersModule.Initialize();

            var soundsModule = new SoundsModule.SoundsModule();
            soundsModule.Initialize();

            var instructionsModule = new InstructionsModule.InstructionsModule();
            instructionsModule.Initialize();

            var settingsModule = new SettingsModule.SettingsModule();
            settingsModule.Initialize();
        }
    }
}