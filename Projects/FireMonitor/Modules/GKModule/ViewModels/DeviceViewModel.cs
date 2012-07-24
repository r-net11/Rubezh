using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class DeviceViewModel : TreeBaseViewModel<DeviceViewModel>
	{
		public XDevice Device { get; private set; }
		public XDeviceState DeviceState { get; private set; }
		public DeviceCommandsViewModel DeviceCommandsViewModel { get; private set; }

		public DeviceViewModel(XDevice device, ObservableCollection<DeviceViewModel> sourceDevices)
		{
			DeviceCommandsViewModel = new DeviceCommandsViewModel(device);
			ShowPlanCommand = new RelayCommand(OnShowPlan, CanShowOnPlan);
			DisableCommand = new RelayCommand(OnDisable, CanDisable);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);

			Source = sourceDevices;
			Device = device;

			DeviceState = XManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == Device.UID);
			if (DeviceState != null)
			{
				DeviceState.StateChanged += new System.Action(OnStateChanged);
				OnStateChanged();
			}
			else
			{
				string deviceName = Device.PresentationAddressDriver;
				string errorText = "Ошибка при сопоставлении устройства с его состоянием:\n" + deviceName;
				Logger.Warn(errorText);
			}
		}

		void OnStateChanged()
		{
			if (DeviceState.States == null)
			{
				Logger.Error("DeviceViewModel.OnStateChanged: DeviceState.States = null");
				return;
			}

			OnPropertyChanged("StateType");
			OnPropertyChanged("DeviceState");
			OnPropertyChanged("DeviceState.States");
		}

		public bool CanShowOnPlan()
		{
			return false;
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				if (plan.ElementDevices.Any(x => x.DeviceUID == Device.UID))
				{
					return true;
				}
			}
			return false;
		}

		public RelayCommand ShowPlanCommand { get; private set; }
		void OnShowPlan()
		{
			ServiceFactory.Events.GetEvent<ShowDeviceOnPlanEvent>().Publish(Device.UID);
		}

		public bool CanDisable()
		{
			return XManager.CanDisable(DeviceState);
		}

		public RelayCommand DisableCommand { get; private set; }
		void OnDisable()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				XManager.ChangeDisabled(DeviceState);
			}
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			ServiceFactory.Events.GetEvent<ShowXDeviceDetailsEvent>().Publish(Device.UID);
		}
	}
}