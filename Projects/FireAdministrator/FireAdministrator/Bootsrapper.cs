using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using System.Windows;
using Infrastructure.Common;
using FiresecClient;
using Infrastructure;


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
            //LoginScreen loginScreen = new LoginScreen();
            //loginScreen.ShowDialog();
            //if (loginScreen.UserName == null)
            //    return;

            FiresecManager.Start("adm", "");

            RegisterServices();

            App.Current.MainWindow = (Window)this.Shell;
            App.Current.MainWindow.Show();
        }

        void RegisterServices()
        {
            ServiceFactory.RegisterType<IResourceService, ResourceService>();
            ServiceFactory.RegisterInstance<ILayoutService>(new LayoutService());
            ServiceFactory.RegisterType<IUserDialogService, UserDialogService>();
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();

            ModuleCatalog moduleCatalog = (ModuleCatalog)this.ModuleCatalog;
            moduleCatalog.AddModule(typeof(DevicesModule.DevicesModule));
            moduleCatalog.AddModule(typeof(LibraryModule.LibraryModule));
            moduleCatalog.AddModule(typeof(PlansModule.PlansModule));
            moduleCatalog.AddModule(typeof(SecurityModule.SequrityModule));
            moduleCatalog.AddModule(typeof(JournalModule.JournalModule));
            moduleCatalog.AddModule(typeof(SoundsModule.SoundsModule));
        }
    }
}
