using System;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using XFiresecAPI;
using Controls.Converters;
using Infrastructure.Common.TreeList;

namespace GKModule.ViewModels
{
    public class DeviceViewModel : TreeItemViewModel<DeviceViewModel>
	{
		public XDevice Device { get; private set; }
        public XDeviceState DeviceState { get; private set; }
        public DeviceStateViewModel DeviceStateViewModel { get; private set; }
		public DeviceCommandsViewModel DeviceCommandsViewModel { get; private set; }

		public DeviceViewModel(XDevice device)
		{
			Device = device;
            DeviceState = Device.DeviceState;
            DeviceStateViewModel = new DeviceStateViewModel(DeviceState);
            DeviceState.StateChanged += new Action(OnStateChanged);
			OnStateChanged();

            DeviceCommandsViewModel = new DeviceCommandsViewModel(DeviceState);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
		}

		void OnStateChanged()
		{
			OnPropertyChanged("DeviceState");
            OnPropertyChanged("DeviceStateViewModel");
		}

		public string PresentationZone
		{
			get { return XManager.GetPresentationZone(Device); }
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			ShowOnPlanHelper.ShowDevice(Device);
		}
		public bool CanShowOnPlan()
		{
			return ShowOnPlanHelper.CanShowDevice(Device);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			ServiceFactory.Events.GetEvent<ShowXDeviceDetailsEvent>().Publish(Device.UID);
		}

        public bool IsBold { get; set; }
	}
}