using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.TreeList;
using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class GuardZoneDeviceViewModel : TreeNodeViewModel<ZoneDeviceViewModel>
	{
		public GKGuardZoneDevice GuardZoneDevice { get; private set; }
		public bool IsCodeReader { get; private set; }

		public GuardZoneDeviceViewModel(GKGuardZoneDevice guardZoneDevice)
		{
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			GuardZoneDevice = guardZoneDevice;

			ActionTypes = new ObservableCollection<GKGuardZoneDeviceActionType>();
			switch (guardZoneDevice.Device.DriverType)
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
			if (!ActionTypes.Contains(SelectedActionType))
				GuardZoneDevice.ActionType = ActionTypes.FirstOrDefault();

			IsCodeReader = guardZoneDevice.Device.Driver.IsCardReaderOrCodeReader;
		}

		public string PresentationZone
		{
			get
			{
				if (GuardZoneDevice.Device.Driver.HasLogic)
					return GKManager.GetPresentationZoneOrLogic(GuardZoneDevice.Device);
				return null;
			}
		}

		public ObservableCollection<GKGuardZoneDeviceActionType> ActionTypes { get; private set; }

		public GKGuardZoneDeviceActionType SelectedActionType
		{
			get { return GuardZoneDevice.ActionType; }
			set
			{
				GuardZoneDevice.ActionType = value;
				OnPropertyChanged(() => SelectedActionType);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public void Clear()
		{
			GuardZoneDevice = new GKGuardZoneDevice { Device = GuardZoneDevice.Device, DeviceUID = GuardZoneDevice.DeviceUID };
			if (!ActionTypes.Contains(SelectedActionType))
				GuardZoneDevice.ActionType = ActionTypes.FirstOrDefault();
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			var codeReaderDetailsViewModel = new CodeReaderDetailsViewModel(GuardZoneDevice.CodeReaderSettings);
			if (DialogService.ShowModalWindow(codeReaderDetailsViewModel))
			{
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
	}
}