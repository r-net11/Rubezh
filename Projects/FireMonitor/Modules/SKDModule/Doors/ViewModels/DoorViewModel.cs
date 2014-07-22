using System;
using System.Linq;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;
using Infrastructure.Events;

namespace SKDModule.ViewModels
{
	public class DoorViewModel : BaseViewModel
	{
		public SKDDoor Door { get; private set; }
		public SKDDevice InDevice { get; private set; }
		public SKDDevice OutDevice { get; private set; }
		public SKDDoorState State
		{
			get { return Door.State; }
		}

		public DoorViewModel(SKDDoor door)
		{
			Door = door;
			InDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == Door.InDeviceUID);
			OutDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == Door.OutDeviceUID);
			State.StateChanged += new Action(OnStateChanged);
			OnStateChanged();

			OpenCommand = new RelayCommand(OnOpen, CanOpen);
			CloseCommand = new RelayCommand(OnClose, CanClose);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
		}

		void OnStateChanged()
		{
			OnPropertyChanged("State");
			OnPropertyChanged("DoorStateViewModel");
		}

		public string PresentationName
		{
			get { return Door.Name; }
		}

		public string PresentationDescription
		{
			get { return Door.Description; }
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			ShowOnPlanHelper.ShowSKDDoor(Door);
		}
		public bool CanShowOnPlan()
		{
			return ShowOnPlanHelper.CanShowSKDDoor(Door);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		private void OnShowProperties()
		{
			DialogService.ShowWindow(new DoorDetailsViewModel(Door));
		}

		public RelayCommand OpenCommand { get; private set; }
		void OnOpen()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var device = SKDManager.Devices.FirstOrDefault(x => x.UID == Door.InDeviceUID);
				if (device != null)
				{
					var result = FiresecManager.FiresecService.SKDOpenDevice(device);
					if (result.HasError)
					{
						MessageBoxService.ShowWarning(result.Error);
					}
				}
			}
		}
		bool CanOpen()
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_ControlDevices);
		}

		public RelayCommand CloseCommand { get; private set; }
		void OnClose()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var device = SKDManager.Devices.FirstOrDefault(x => x.UID == Door.InDeviceUID);
				if (device != null)
				{
					var result = FiresecManager.FiresecService.SKDCloseDevice(device);
					if (result.HasError)
					{
						MessageBoxService.ShowWarning(result.Error);
					}
				}
			}
		}
		bool CanClose()
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_ControlDevices);
		}

		public bool IsBold { get; set; }
	}
}