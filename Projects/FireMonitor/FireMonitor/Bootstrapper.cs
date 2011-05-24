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

            PlansModule.PlansModule plansModule = new PlansModule.PlansModule();
            plansModule.Initialize();

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
            moduleCatalog.AddModule(typeof(AlarmModule.AlarmModule));
            //moduleCatalog.AddModule(typeof(PlansModule.PlansModule));
            moduleCatalog.AddModule(typeof(JournalModule.JournalModule));
            moduleCatalog.AddModule(typeof(DevicesModule.DevicesModule));
            moduleCatalog.AddModule(typeof(ReportsModule.ReportsModule));
            moduleCatalog.AddModule(typeof(CallModule.CallModule));
        }

        void StartFiresecClient()
        {
            FiresecManager.Start();
            FiresecManager.CurrentStates.DeviceStateChanged += CurrentStates_DeviceStateChanged;
            FiresecManager.CurrentStates.DeviceParametersChanged += new Action<string>(CurrentStates_DeviceParametersChanged);
            FiresecManager.CurrentStates.ZoneStateChanged += new Action<string>(CurrentStates_ZoneStateChanged);
            CurrentStates.NewJournalEvent += new Action<Firesec.ReadEvents.journalType>(CurrentStates_NewJournalEvent);
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

        void CurrentStates_NewJournalEvent(Firesec.ReadEvents.journalType journalItem)
        {
            //Alarm alarm = new Alarm();
            //alarm.AlarmType = AlarmType.Service;
            //alarm.Name = journalItem.EventDesc;
            //ServiceFactory.Events.GetEvent<AlarmAddedEvent>().Publish(alarm);

            //Trace.WriteLine("New Event");
        }
    }
}
