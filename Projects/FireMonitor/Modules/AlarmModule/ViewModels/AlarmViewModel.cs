using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using AlarmModule.Events;
using Infrastructure.Events;
using FiresecClient;

namespace AlarmModule.ViewModels
{
    public class AlarmViewModel : RegionViewModel
    {
        public AlarmViewModel()
        {
            ResetCommand = new RelayCommand(OnReset);
            ShowOnPlanCommand = new RelayCommand(OnShowOnPlan);
        }

        public Alarm alarm;
        public AlarmType AlarmType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public void Initialize(Alarm alarm)
        {
            this.alarm = alarm;
            AlarmType = alarm.AlarmType;
            Name = alarm.Name;
            Description = alarm.Description;
        }

        public RelayCommand ResetCommand { get; private set; }
        void OnReset()
        {
            if (alarm.PanelPath != null)
            {
                Device device = FiresecManager.CurrentConfiguration.AllDevices.FirstOrDefault(x => x.Path == alarm.PanelPath);

                Firesec.Metadata.drvType driver = FiresecManager.CurrentConfiguration.Metadata.drv.FirstOrDefault(x => x.id == device.DriverId);
                if (driver.state != null)
                {
                    foreach (Firesec.Metadata.stateType state in driver.state)
                    {
                        if ((state.@class == alarm.ClassId) && (state.manualReset == "1"))
                        {
                            FiresecManager.ResetState(device, state.name);
                        }
                    }
                }
            }

            ServiceFactory.Events.GetEvent<ResetAlarmEvent>().Publish(alarm);
            ServiceFactory.Layout.ShowAlarm(null);
        }

        public RelayCommand ShowOnPlanCommand { get; private set; }
        void OnShowOnPlan()
        {
            ServiceFactory.Events.GetEvent<ShowPlanEvent>().Publish(null);
        }

        public override void Dispose()
        {
        }
    }
}
