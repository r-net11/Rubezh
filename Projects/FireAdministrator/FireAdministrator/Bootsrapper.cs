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

            ResourceDictionary rd = new ResourceDictionary() { Source = new System.Uri("pack://application:,,,/Infrastructure.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;component/Themes/DataGridStyle.xaml") };
            Application.Current.Resources.MergedDictionaries.Add(rd);
        

            InitializeModules();

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

            return;

            ModuleCatalog moduleCatalog = (ModuleCatalog)this.ModuleCatalog;
            moduleCatalog.AddModule(typeof(DevicesModule.DevicesModule));
            moduleCatalog.AddModule(typeof(LibraryModule.LibraryModule));
            moduleCatalog.AddModule(typeof(PlansModule.PlansModule));
            moduleCatalog.AddModule(typeof(SecurityModule.SequrityModule));
            moduleCatalog.AddModule(typeof(JournalModule.JournalModule));
            moduleCatalog.AddModule(typeof(SoundsModule.SoundsModule));
        }

        void InitializeModules()
        {
            DevicesModule.DevicesModule devicesModule = new DevicesModule.DevicesModule();
            devicesModule.Initialize();

            LibraryModule.LibraryModule libraryModule = new LibraryModule.LibraryModule();
            libraryModule.Initialize();

            PlansModule.PlansModule plansModule = new PlansModule.PlansModule();
            plansModule.Initialize();

            SecurityModule.SequrityModule sequrityModule = new SecurityModule.SequrityModule();
            sequrityModule.Initialize();

            JournalModule.JournalModule journalModule = new JournalModule.JournalModule();
            journalModule.Initialize();

            SoundsModule.SoundsModule soundsModule = new SoundsModule.SoundsModule();
            soundsModule.Initialize();
        }
    }
}
