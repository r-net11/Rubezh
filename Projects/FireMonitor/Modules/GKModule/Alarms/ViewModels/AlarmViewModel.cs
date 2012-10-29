using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using Infrastructure.Events;
using Infrastructure;

namespace GKModule.ViewModels
{
	public class AlarmViewModel : BaseViewModel
	{
		public Alarm Alarm { get; private set; }

		public AlarmViewModel(Alarm alarm)
		{
            ShowObjectCommand = new RelayCommand(OnShowObject);
            ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			Alarm = alarm;
		}

        public RelayCommand ShowObjectCommand { get; private set; }
        void OnShowObject()
        {
            if (Alarm.Device != null)
            {
                ServiceFactory.Events.GetEvent<ShowXDeviceEvent>().Publish(Alarm.Device.UID);
            }
            if (Alarm.Zone != null)
            {
                ServiceFactory.Events.GetEvent<ShowXZoneEvent>().Publish(Alarm.Zone.UID);
            }
            if (Alarm.Direction != null)
            {
                ServiceFactory.Events.GetEvent<ShowXDirectionEvent>().Publish(Alarm.Direction.UID);
            }
        }

        public RelayCommand ShowOnPlanCommand { get; private set; }
        void OnShowOnPlan()
        {
            if (Alarm.Device != null)
            {
                ShowOnPlanHelper.ShowDevice(Alarm.Device);
            }
            if (Alarm.Zone != null)
            {
                ShowOnPlanHelper.ShowZone(Alarm.Zone);
            }
        }
        bool CanShowOnPlan()
        {
            if (Alarm.Device != null)
            {
                return ShowOnPlanHelper.CanShowDevice(Alarm.Device);
            }
            if (Alarm.Zone != null)
            {
                return ShowOnPlanHelper.CanShowZone(Alarm.Zone);
            }
            return false;
        }
	}
}