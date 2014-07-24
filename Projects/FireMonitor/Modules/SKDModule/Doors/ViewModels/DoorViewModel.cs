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
using FiresecAPI.GK;

namespace SKDModule.ViewModels
{
	public class DoorViewModel : BaseViewModel
	{
		public SKDDoor Door { get; private set; }
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
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);

			ShowInDeviceCommand = new RelayCommand(OnShowInDevice, CanShowInDevice);
			ShowOutDeviceCommand = new RelayCommand(OnShowOutDevice, CanShowOutDevice);
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

		#region Devices
		public SKDDevice InDevice { get; private set; }
		public bool HasInDevice
		{
			get { return InDevice != null; }
		}
		public RelayCommand ShowInDeviceCommand { get; private set; }
		void OnShowInDevice()
		{
			ServiceFactory.Events.GetEvent<ShowSKDDeviceEvent>().Publish(InDevice.UID);
		}
		bool CanShowInDevice()
		{
			return InDevice != null;
		}

		public SKDDevice OutDevice { get; private set; }
		public bool HasOutDevice
		{
			get { return OutDevice != null; }
		}
		public RelayCommand ShowOutDeviceCommand { get; private set; }
		void OnShowOutDevice()
		{
			ServiceFactory.Events.GetEvent<ShowSKDDeviceEvent>().Publish(OutDevice.UID);
		}
		bool CanShowOutDevice()
		{
			return OutDevice != null;
		}
		#endregion

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			ShowOnPlanHelper.ShowSKDDoor(Door);
		}
		public bool CanShowOnPlan()
		{
			return ShowOnPlanHelper.CanShowSKDDoor(Door);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			var showSKDArchiveEventArgs = new ShowArchiveEventArgs()
			{
				Door = Door
			};
			ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(showSKDArchiveEventArgs);
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
				var result = FiresecManager.FiresecService.SKDOpenDoor(Door.UID);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		bool CanOpen()
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_ControlDevices) && Door.State.StateClass != XStateClass.On && Door.State.StateClass != XStateClass.ConnectionLost;
		}

		public RelayCommand CloseCommand { get; private set; }
		void OnClose()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDCloseDoor(Door.UID);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		bool CanClose()
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_ControlDevices) && Door.State.StateClass != XStateClass.Off && Door.State.StateClass != XStateClass.ConnectionLost;
		}

		public bool IsBold { get; set; }
	}
}