using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;

using System.Windows;
using Infrastructure;
using ClientApi;
using Infrastructure.Events;

namespace PrismApp1
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

            App.Current.MainWindow = (Window)this.Shell;
            App.Current.MainWindow.Show();
        }

        void RegisterServices()
        {
            ServiceFactory.RegisterType<IResourceService, ResourceService>();
            ServiceFactory.RegisterInstance<ILayoutService>(new LayoutService());
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();

            ModuleCatalog moduleCatalog = (ModuleCatalog)this.ModuleCatalog;
            moduleCatalog.AddModule(typeof(AlarmModule.AlarmModule));
            moduleCatalog.AddModule(typeof(PlansModule.PlansModule));
            moduleCatalog.AddModule(typeof(JournalModule.JournalModule));
            moduleCatalog.AddModule(typeof(DevicesModule.DevicesModule));
        }

        void StartFiresecClient()
        {
            ClientManager.Start();
            ServiceClient.CurrentStates.DeviceStateChanged += CurrentStates_DeviceStateChanged;
            ServiceClient.CurrentStates.DeviceParametersChanged += new Action<string>(CurrentStates_DeviceParametersChanged);
            ServiceClient.CurrentStates.ZoneStateChanged += new Action<string>(CurrentStates_ZoneStateChanged);
            ServiceClient.CurrentStates.NewJournalEvent += new Action<Firesec.ReadEvents.journalType>(CurrentStates_NewJournalEvent);
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
