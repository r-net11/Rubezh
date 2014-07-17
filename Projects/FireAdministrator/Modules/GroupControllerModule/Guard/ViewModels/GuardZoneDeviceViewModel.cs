using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.TreeList;
using System.Collections.ObjectModel;
using System;
using System.Linq;
using Infrastructure;

namespace GKModule.ViewModels
{
	public class GuardZoneDeviceViewModel : TreeNodeViewModel<ZoneDeviceViewModel>
	{
		public XGuardZoneDevice GuardZoneDevice { get; private set; }

		public GuardZoneDeviceViewModel(XGuardZoneDevice guardZoneDevice)
		{
			GuardZoneDevice = guardZoneDevice;

			ActionTypes = new ObservableCollection<XGuardZoneDeviceActionType>();
			switch (guardZoneDevice.Device.DriverType)
			{
				case XDriverType.RSR2_GuardDetector:
					ActionTypes.Add(XGuardZoneDeviceActionType.SetAlarm);
					break;

				case XDriverType.RSR2_AM_1:
				case XDriverType.RSR2_HandDetector:
					ActionTypes.Add(XGuardZoneDeviceActionType.SetGuard);
					ActionTypes.Add(XGuardZoneDeviceActionType.ResetGuard);
					ActionTypes.Add(XGuardZoneDeviceActionType.SetAlarm);
					break;
			}
			if (!ActionTypes.Contains(SelectedActionType))
				GuardZoneDevice.ActionType = ActionTypes.FirstOrDefault();
		}

		public bool IsBold { get; set; }

		public ObservableCollection<XGuardZoneDeviceActionType> ActionTypes { get; private set; }

		public XGuardZoneDeviceActionType SelectedActionType
		{
			get { return GuardZoneDevice.ActionType; }
			set
			{
				GuardZoneDevice.ActionType = value;
				OnPropertyChanged(() => SelectedActionType);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
	}
}