using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;

using System.Windows;
using Infrastructure;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Events;

namespace FireMonitor
{
    public class Bootstrapper : UnityBootstrapper
    {
        public Bootstrapper()
        {
            ResourceDictionary rd = new ResourceDictionary() { Source = new System.Uri("pack://application:,,,/Infrastructure.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;component/Themes/DataGridStyle.xaml") };
            Application.Current.Resources.MergedDictionaries.Add(rd);
        }

        protected override System.Windows.DependencyObject CreateShell()
        {
            ShellView shellView = new ShellView();
            ServiceFactory.RegisterInstance(shellView);
            return shellView;
        }

        public static bool Connect()
        {
            LoginView loginScreen = new LoginView();
            loginScreen.ShowDialog();
            if (loginScreen.IsLoggedIn == false)
            {
                return false;
            }

            //if (FiresecManager.CurrentPermissions.Any(x => x.PermissionType == FiresecClient.Models.PermissionType.Oper_Login) == false)
            //{
            //    MessageBox.Show("Нет прав на работу с программой");
            //    FiresecManager.Disconnect();
            //    return false;
            //}

            return true;
        }

        protected override void InitializeShell()
        {
            bool result = Connect();
            if (result == false)
            {
                return;
            }

            RegisterServices();

            InitializeFiresecClient();

            InitializeKnownModules();

            App.Current.MainWindow = (Window)this.Shell;
            App.Current.MainWindow.Show();
        }

        void RegisterServices()
        {
            ServiceFactory.RegisterType<IResourceService, ResourceService>();
            ServiceFactory.RegisterInstance<ILayoutService>(new LayoutService());
            ServiceFactory.RegisterType<IUserDialogService, UserDialogService>();
            ServiceFactory.RegisterType<ISecurityService, SecurityService>();
        }

        void InitializeKnownModules()
        {
            DevicesModule.DevicesModule devicesModule = new DevicesModule.DevicesModule();
            devicesModule.Initialize();

            PlansModule.PlansModule plansModule = new PlansModule.PlansModule();
            plansModule.Initialize();

            JournalModule.JournalModule journalModule = new JournalModule.JournalModule();
            journalModule.Initialize();

            AlarmModule.AlarmModule alarmModule = new AlarmModule.AlarmModule();
            alarmModule.Initialize();

            ReportsModule.ReportsModule reportsModule = new ReportsModule.ReportsModule();
            reportsModule.Initialize();

            CallModule.CallModule callModule = new CallModule.CallModule();
            callModule.Initialize();
        }

        void InitializeFiresecClient()
        {
            FiresecManager.States.DeviceStateChanged += CurrentStates_DeviceStateChanged;
            FiresecManager.States.DeviceParametersChanged += new Action<string>(CurrentStates_DeviceParametersChanged);
            FiresecManager.States.ZoneStateChanged += new Action<string>(CurrentStates_ZoneStateChanged);
        }

        void CurrentStates_DeviceStateChanged(string obj)
        {
            ServiceFactory.Events.GetEvent<DeviceStateChangedEvent>().Publish(obj);
        }

        void CurrentStates_DeviceParametersChanged(string obj)
        {
            ServiceFactory.Events.GetEvent<DeviceParametersChangedEvent>().Publish(obj);
        }

        void CurrentStates_ZoneStateChanged(string obj)
        {
            ServiceFactory.Events.GetEvent<ZoneStateChangedEvent>().Publish(obj);
        }
    }
}
