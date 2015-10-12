using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.TreeList;
using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;

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

			IsCodeReader = guardZoneDevice.Device.DriverType == GKDriverType.RSR2_CodeReader || guardZoneDevice.Device.DriverType == GKDriverType.RSR2_CardReader;
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