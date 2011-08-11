using System.Linq;
using System.Windows;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Microsoft.Practices.Prism.UnityExtensions;

namespace FireAdministrator
{
    public class Bootsrapper : UnityBootstrapper
    {
        protected override System.Windows.DependencyObject CreateShell()
        {
            ShellView shellView = new ShellView();
            ServiceFactory.RegisterInstance(shellView);
            return shellView;
        }

        protected override void InitializeShell()
        {
            LoginScreen loginScreen = new LoginScreen();
            loginScreen.ShowDialog();
            if (loginScreen.IsLoggedIn == false)
            {
                return;
            }

            if (FiresecManager.CurrentPermissions.Any(x => x.PermissionType == PermissionType.Adm_ViewConfig) == false)
            {
                MessageBox.Show("Нет прав на работу с программой");
                FiresecManager.Disconnect();
                return;
            }

            RegisterServices();

            ResourceDictionary rd = new ResourceDictionary() { Source = new System.Uri("pack://application:,,,/Infrastructure.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;component/Themes/DataGridStyle.xaml") };
            Application.Current.Resources.MergedDictionaries.Add(rd);

            InitializeKnownModules();

            App.Current.MainWindow = (Window) this.Shell;
            App.Current.MainWindow.Show();
        }

        void RegisterServices()
        {
            ServiceFactory.RegisterType<IResourceService, ResourceService>();
            ServiceFactory.RegisterInstance<ILayoutService>(new LayoutService());
            ServiceFactory.RegisterType<IUserDialogService, UserDialogService>();
        }

        void InitializeKnownModules()
        {
            DevicesModule.DevicesModule devicesModule = new DevicesModule.DevicesModule();
            devicesModule.Initialize();

            LibraryModule.LibraryModule libraryModule = new LibraryModule.LibraryModule();
            libraryModule.Initialize();

            PlansModule.PlansModule plansModule = new PlansModule.PlansModule();
            plansModule.Initialize();

            SecurityModule.SequrityModule sequrityModule = new SecurityModule.SequrityModule();
            sequrityModule.Initialize();

            FiltersModule.FilterModule filtersModule = new FiltersModule.FilterModule();
            filtersModule.Initialize();

            SoundsModule.SoundsModule soundsModule = new SoundsModule.SoundsModule();
            soundsModule.Initialize();

            InstructionsModule.InstructionsModule instructionsModule = new InstructionsModule.InstructionsModule();
            instructionsModule.Initialize();
        }
    }
}
