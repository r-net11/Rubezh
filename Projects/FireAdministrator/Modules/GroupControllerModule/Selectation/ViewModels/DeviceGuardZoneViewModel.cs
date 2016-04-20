using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class DeviceGuardZoneViewModel : BaseViewModel
	{
		public GKDeviceGuardZone DeviceGuardZone { get; private set; }
		public bool IsCodeReader { get; private set; }
		public int No { get; private set; }
		public string Name { get; private set; }
		public string Description { get; private set; }

		public DeviceGuardZoneViewModel(GKDeviceGuardZone deviceGuardZone, GKDevice device)
		{
			DeviceGuardZone = deviceGuardZone;
			if (device != null)
				IsCodeReader = device.Driver.IsCardReaderOrCodeReader;
			No = deviceGuardZone.GuardZone.No;
			Name = deviceGuardZone.GuardZone.Name;
			Description = deviceGuardZone.GuardZone.Description;
			ActionTypes = new ObservableCollection<GKGuardZoneDeviceActionType>();
			if (device != null)
			switch (device.DriverType)
			{
				case GKDriverType.RSR2_GuardDetector:
				case GKDriverType.RSR2_GuardDetectorSound:
				case GKDriverType.RSR2_HandGuardDetector:
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
			if (deviceGuardZone.ActionType == null || !ActionTypes.Contains(deviceGuardZone.ActionType.Value))
				SelectedActionType = ActionTypes.FirstOrDefault();
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
