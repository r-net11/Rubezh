using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.TreeList;
using System.Collections.ObjectModel;
using System;
using System.Linq;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;

namespace GKModule.ViewModels
{
	public class GuardZoneDeviceViewModel : TreeNodeViewModel<ZoneDeviceViewModel>
	{
		public XGuardZoneDevice GuardZoneDevice { get; private set; }
		public bool IsCodeReader { get; private set; }

		public GuardZoneDeviceViewModel(XGuardZoneDevice guardZoneDevice)
		{
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
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

			IsCodeReader = guardZoneDevice.Device.DriverType == XDriverType.RSR2_CodeReader;
		}

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