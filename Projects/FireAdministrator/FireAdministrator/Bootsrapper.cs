using System.Linq;
using System.Windows;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Microsoft.Practices.Prism.UnityExtensions;

namespace FireAdministrator
{
    public class Bootsrapper
    {
        public void Initialize()
        {
            RegisterServices();

            var preLoadWindow = new PreLoadWindow();
            var loginScreen = new LoginScreen();
            loginScreen.ShowDialog();
            if (loginScreen.IsLoggedIn)
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