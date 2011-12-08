using System.Linq;
using AlarmModule.Events;
using FireMonitor.ViewModels;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace FireMonitor
{
    public class Bootstrapper
    {
        public void Initialize()
        {
            RegisterServices();
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));

            var preLoadWindow = new PreLoadWindow();

            var loginViewModel = new LoginViewModel(LoginViewModel.PasswordViewType.Connect);
            if (ServiceFactory.UserDialogs.ShowModalWindow(loginViewModel))
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

                    var ShellView = new ShellView();
                    ServiceFactory.ShellView = ShellView;

                    InitializeKnownModules();

                    App.Current.MainWindow = ShellView;
                    App.Current.MainWindow.Show();

                    ServiceFactory.Events.GetEvent<ShowAlarmsEvent>().Publish(null);
                }
                preLoadWindow.Close();
            }
        }

        static void RegisterServices()
        {
            ServiceFactory.Initialize(new LayoutService(), new UserDialogService(), new ResourceService(), new SecurityService());
        }

        void InitializeKnownModules()
        {
            var devicesModule = new DevicesModule.DevicesModule();
            //var journalModule = new JournalModule.JournalModule();
            var alarmModule = new AlarmModule.AlarmModule();
            var reportsModule = new ReportsModule.ReportsModule();
            var callModule = new CallModule.CallModule();
            var plansModule = new PlansModule.PlansModule();
        }
    }
}