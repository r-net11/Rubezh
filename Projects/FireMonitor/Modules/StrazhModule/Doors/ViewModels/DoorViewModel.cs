using System;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using StrazhModule.Doors;

namespace StrazhModule.ViewModels
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
			State.StateChanged -= new Action(OnStateChanged);
			State.StateChanged += new Action(OnStateChanged);
			OnStateChanged();

			OpenCommand = new RelayCommand(OnOpen, CanOpen);
			CloseCommand = new RelayCommand(OnClose, CanClose);
			OpenForeverCommand = new RelayCommand(OnOpenForever, CanOpenForever);
			CloseForeverCommand = new RelayCommand(OnCloseForever, CanCloseForever);
			DoorAccessStateNormalCommand = new RelayCommand(OnDoorAccessStateNormal, CanDoorAccessStateNormal);
			DoorAccessStateCloseAlwaysCommand = new RelayCommand(OnDoorAccessStateCloseAlways, CanDoorAccessStateCloseAlways);
			DoorAccessStateOpenAlwaysCommand = new RelayCommand(OnDoorAccessStateOpenAlways, CanDoorAccessStateOpenAlways);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);

			ShowInDeviceCommand = new RelayCommand(OnShowInDevice, CanShowInDevice);
			ShowOutDeviceCommand = new RelayCommand(OnShowOutDevice, CanShowOutDevice);

			ClearPromptWarningCommand = new RelayCommand(OnClearPromptWarning, CanClearPromptWarning);
		}

		void OnStateChanged()
		{
			OnPropertyChanged(() => State);
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
				SKDDoor = Door
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
			DoorCommander.Open(Door);
		}
		bool CanOpen()
		{
			return DoorCommander.CanOpen(Door);
		}

		public RelayCommand CloseCommand { get; private set; }
		void OnClose()
		{
			DoorCommander.Close(Door);
		}
		bool CanClose()
		{
			return DoorCommander.CanClose(Door);
		}

		public RelayCommand OpenForeverCommand { get; private set; }
		void OnOpenForever()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDOpenDoorForever(Door);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		bool CanOpenForever()
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Doors_Control) && State.StateClass != XStateClass.On && State.StateClass != XStateClass.ConnectionLost;
		}

		public RelayCommand CloseForeverCommand { get; private set; }
		void OnCloseForever()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDCloseDoorForever(Door);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		bool CanCloseForever()
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Doors_Control) && State.StateClass != XStateClass.Off && State.StateClass != XStateClass.ConnectionLost;
		}

		public RelayCommand DoorAccessStateNormalCommand { get; private set; }
		void OnDoorAccessStateNormal()
		{
			DoorCommander.SetAccessStateToNormal(Door);
		}
		bool CanDoorAccessStateNormal()
		{
			return DoorCommander.CanSetAccessStateToNormal(Door);
		}

		public RelayCommand DoorAccessStateCloseAlwaysCommand { get; private set; }
		void OnDoorAccessStateCloseAlways()
		{
			DoorCommander.SetAccessStateToCloseAlways(Door);
		}
		bool CanDoorAccessStateCloseAlways()
		{
			return DoorCommander.CanSetAccessStateToCloseAlways(Door);
		}

		public RelayCommand DoorAccessStateOpenAlwaysCommand { get; private set; }
		void OnDoorAccessStateOpenAlways()
		{
			DoorCommander.SetAccessStateToOpenAlways(Door);
		}
		bool CanDoorAccessStateOpenAlways()
		{
			return DoorCommander.CanSetAccessStateToOpenAlways(Door);
		}

		public RelayCommand ClearPromptWarningCommand { get; private set; }
		private void OnClearPromptWarning()
		{
			DoorCommander.ClearPromptWarning(Door);
		}
		private bool CanClearPromptWarning()
		{
			return DoorCommander.CanClearPromptWarning(Door);
		}

		public bool IsBold { get; set; }
	}
}