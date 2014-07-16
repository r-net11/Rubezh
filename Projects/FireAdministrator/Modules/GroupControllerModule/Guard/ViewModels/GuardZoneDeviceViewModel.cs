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
			ActionTypes = new ObservableCollection<XGuardZoneDeviceActionType>(Enum.GetValues(typeof(XGuardZoneDeviceActionType)).Cast<XGuardZoneDeviceActionType>());
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