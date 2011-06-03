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
        protected override System.Windows.DependencyObject CreateShell()
        {
            ShellView shellView = new ShellView();
            ServiceFactory.RegisterInstance(shellView);
            return shellView;
        }

        protected override void InitializeShell()
        {
            RegisterServices();

            StartFiresecClient();

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
            moduleCatalog.AddModule(typeof(AlarmModule.AlarmModule));
            moduleCatalog.AddModule(typeof(PlansModule.PlansModule));
            moduleCatalog.AddModule(typeof(JournalModule.JournalModule));
            moduleCatalog.AddModule(typeof(DevicesModule.DevicesModule));
            moduleCatalog.AddModule(typeof(ReportsModule.ReportsModule));
            moduleCatalog.AddModule(typeof(CallModule.CallModule));
        }

        void InitializeModules()
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

        void StartFiresecClient()
        {
            FiresecManager.Start();
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
