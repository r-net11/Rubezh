using System.Linq;
using System.Windows;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Microsoft.Practices.Prism.UnityExtensions;

namespace FireMonitor
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override System.Windows.DependencyObject CreateShell()
        {
            var shellView = new ShellView();
            ServiceFactory.RegisterInstance(shellView);

            return shellView;
        }

        protected override void InitializeShell()
        {
            RegisterServices();
            if (ServiceFactory.Get<ISecurityService>().Connect())
            {
                var preLoadWindow = new PreLoadWindow();
                preLoadWindow.Show();
                FiresecManager.SelectiveFetch();

                if (FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Oper_Login) == false)
                {
                    DialogBox.DialogBox.Show("Нет прав на работу с программой");
                    FiresecManager.Disconnect();
                    preLoadWindow.Close();
                }
                else
                {
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
            ServiceFactory.RegisterType<ISecurityService, SecurityService>();
        }

        void InitializeKnownModules()
        {
            var devicesModule = new DevicesModule.DevicesModule();
            devicesModule.Initialize();

            var journalModule = new JournalModule.JournalModule();
            journalModule.Initialize();

            var alarmModule = new AlarmModule.AlarmModule();
            alarmModule.Initialize();

            var reportsModule = new ReportsModule.ReportsModule();
            reportsModule.Initialize();

            var callModule = new CallModule.CallModule();
            callModule.Initialize();

            //var plansModule = new PlansModule.PlansModule();
            //plansModule.Initialize();
        }
    }
}