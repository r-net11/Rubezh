using System.Collections.ObjectModel;
using FiresecAPI.GK;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class DeviceGuardZoneViewModel : BaseViewModel
	{
		public GKDeviceGuardZone DeviceGuardZone { get; private set; }
		GKDevice Device { get; set; }
		public bool IsCodeReader { get; private set; }
		public int No { get; private set; }
		public string Name { get; private set; }
		public string Description { get; private set; }

		public DeviceGuardZoneViewModel(GKDeviceGuardZone deviceGuardZone, GKDevice device)
		{
			Device = device;
			DeviceGuardZone = deviceGuardZone;
			IsCodeReader = device.DriverType == GKDriverType.RSR2_CardReader || device.DriverType == GKDriverType.RSR2_CodeReader;
			No = deviceGuardZone.GuardZone.No;
			Name = deviceGuardZone.GuardZone.Name;
			Description = deviceGuardZone.GuardZone.Description;
			ActionTypes = new ObservableCollection<GKGuardZoneDeviceActionType>();
			switch (device.DriverType)
			{
				case GKDriverType.RSR2_GuardDetector:
				case GKDriverType.RSR2_GuardDetectorSound:
					ActionTypes.Add(GKGuardZoneDeviceActionType.SetAlarm);
					break;

				case GKDriverType.RSR2_AM_1:
				case GKDriverType.RSR2_MAP4:
					ActionTypes.Add(GKGuardZoneDeviceActionType.SetGuard);
					ActionTypes.Add(GKGuardZoneDeviceActionType.ResetGuard);
					ActionTypes.Add(GKGuardZoneDeviceActionType.ChangeGuard);
					ActionTypes.Add(GKGuardZoneDeviceActionType.SetAlarm);
					break;
			}

			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
		}

		public ObservableCollection<GKGuardZoneDeviceActionType> ActionTypes { get; private set; }
		public GKGuardZoneDeviceActionType? SelectedActionType
		{
			get { return DeviceGuardZone.ActionType; }
			set
			{
				DeviceGuardZone.ActionType = value;
				OnPropertyChanged(() => SelectedActionType);
			}
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			var codeReaderDetailsViewModel = new CodeReaderDetailsViewModel(DeviceGuardZone.CodeReaderSettings);
			if (DialogService.ShowModalWindow(codeReaderDetailsViewModel))
			{
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
	}
}
