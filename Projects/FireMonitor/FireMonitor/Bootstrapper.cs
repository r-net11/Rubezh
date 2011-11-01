using System.Linq;
using System.Windows;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Microsoft.Practices.Prism.UnityExtensions;

namespace FireMonitor
{
    public class Bootstrapper
    {
        public void Initialize()
        {
            RegisterServices();

            var preLoadWindow = new PreLoadWindow();

            if (ServiceFactory.SecurityService.Connect())
            {
                preLoadWindow.Show();
                FiresecManager.SelectiveFetch();

                if (FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Oper_Login) == false)
                {
                    DialogBox.DialogBox.Show("Нет прав на работу с программой");
                    FiresecManager.Disconnect();
                }
                else
                {
                    ClientSettings.LoadSettings();
                    InitializeKnownModules();

                    App.Current.MainWindow = ServiceFactory.ShellView;
                    App.Current.MainWindow.Show();
                }
                preLoadWindow.Close();
            }
        }

        static void RegisterServices()
        {
            ServiceFactory.Initialize(new LayoutService(), new UserDialogService(), new ResourceService(), new SecurityService());

            var ShellView = new ShellView();
            ServiceFactory.ShellView = ShellView;
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

            var plansModule = new PlansModule.PlansModule();
            plansModule.Initialize();
        }
    }
}