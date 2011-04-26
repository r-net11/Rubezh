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
            CloseCommand = new RelayCommand(OnClose);
            LeaveCommand = new RelayCommand(OnLeave);
            ConfirmCommand = new RelayCommand(OnConfirm);
        }

        public Alarm alarm;

        public AlarmType AlarmType
        {
            get { return alarm.AlarmType; }
        }

        public string Name
        {
            get { return alarm.Name; }
        }

        public string Description
        {
            get { return alarm.Description; }
        }

        public string Time
        {
            get { return alarm.Time; }
        }

        public void Initialize(Alarm alarm)
        {
            this.alarm = alarm;
        }

        public RelayCommand ResetCommand { get; private set; }
        void OnReset()
        {
            Reset();
            Close();
        }

        public RelayCommand ShowOnPlanCommand { get; private set; }
        void OnShowOnPlan()
        {
            ServiceFactory.Events.GetEvent<ShowPlanEvent>().Publish(null);
        }

        public RelayCommand CloseCommand { get; private set; }
        void OnClose()
        {
            Close();
        }

        public RelayCommand LeaveCommand { get; private set; }
        void OnLeave()
        {
            ServiceFactory.Events.GetEvent<MoveAlarmToEndEvent>().Publish(this);
            Close();
        }

        public RelayCommand ConfirmCommand { get; private set; }
        void OnConfirm()
        {
            Close();
        }

        void Reset()
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
        }

        void Close()
        {
            ServiceFactory.Layout.ShowAlarm(null);
        }

        public override void Dispose()
        {
        }
    }
}
