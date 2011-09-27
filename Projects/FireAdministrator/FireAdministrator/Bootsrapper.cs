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
    public class Bootsrapper : UnityBootstrapper
    {
        protected override System.Windows.DependencyObject CreateShell()
        {
            var shellView = new ShellView();
            ServiceFactory.RegisterInstance(shellView);
            return shellView;
        }

        protected override void InitializeShell()
        {
            var loginScreen = new LoginScreen();
            loginScreen.ShowDialog();
            if (loginScreen.IsLoggedIn)
            {
                var preLoadWindow = new PreLoadWindow();
                preLoadWindow.Show();
                FiresecManager.SelectiveFetch();

                if (FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Adm_ViewConfig) == false)
                {
                    DialogBox.DialogBox.Show("Нет прав на работу с программой");
                    FiresecManager.Disconnect();
                    preLoadWindow.Close();
                }
                else
                {
                    RegisterServices();
                    InitializeKnownModules();
                    preLoadWindow.Close();

                    App.Current.MainWindow = (Window) this.Shell;
                    App.Current.MainWindow.Show();
                }
            }
        }

        static void RegisterServices()
        {
            ServiceFactory.RegisterType<IResourceService, ResourceService>();
            ServiceFactory.RegisterInstance<ILayoutService>(new LayoutService());
            ServiceFactory.RegisterType<IUserDialogService, UserDialogService>();
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