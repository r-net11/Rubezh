using System.Collections.ObjectModel;
using System.Linq;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using XFiresecAPI;
using System;
using System.Collections.Generic;
using FiresecAPI;

namespace GKModule.ViewModels
{
	public class DeviceViewModel : TreeBaseViewModel<DeviceViewModel>
	{
		public XDevice Device { get; private set; }
		public XDeviceState DeviceState { get; private set; }
		public DeviceCommandsViewModel DeviceCommandsViewModel { get; private set; }

		public DeviceViewModel(XDevice device, ObservableCollection<DeviceViewModel> sourceDevices)
		{
			Source = sourceDevices;
			Device = device;
			DeviceState = XManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == Device.UID);
			if (DeviceState != null)
			{
				DeviceState.StateChanged += new Action(OnStateChanged);
				OnStateChanged();
			}
			else
			{
				Logger.Warn("Ошибка при сопоставлении устройства с его состоянием: " + Device.PresentationAddressAndDriver);
				MessageBoxService.Show("Ошибка при сопоставлении устройства с его состоянием");
			}

			DeviceCommandsViewModel = new DeviceCommandsViewModel(DeviceState);
			ShowPlanCommand = new RelayCommand(OnShowPlan, CanShowOnPlan);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
		}

		void OnStateChanged()
		{
			if (DeviceState.States == null)
			{
				Logger.Error("DeviceViewModel.OnStateChanged: DeviceState.States = null");
				return;
			}

			OnPropertyChanged("StateType");
			OnPropertyChanged("States");
		}

		public StateType StateType
		{
			get { return DeviceState.StateType; }
		}

		public List<XStateType> States
		{
			get { return DeviceState.States; }
		}

		public ushort DatabaseNo
		{
			get { return Device.GetDatabaseNo(DatabaseType.Gk); }
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

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			ServiceFactory.Events.GetEvent<ShowXDeviceDetailsEvent>().Publish(Device.UID);
		}
	}
}