using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Microsoft.Practices.Prism;

namespace GKModule.ViewModels
{
	public class DoorViewModel : BaseViewModel
	{
		public GKDoor Door { get; private set; }
		public GKState State
		{
			get { return Door.State; }
		}
		public DoorDetailsViewModel DoorDetailsViewModel { get; private set; }
		public ObservableCollection<DeviceViewModel> Devices { get; private set; }

		public DoorViewModel(GKDoor door)
		{
			Door = door;
			DoorDetailsViewModel = new DoorDetailsViewModel(door);
			State.StateChanged -= OnStateChanged;
			State.StateChanged += OnStateChanged;
			OnStateChanged();

			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			ShowOnPlanOrPropertiesCommand = new RelayCommand(OnShowOnPlanOrProperties);
			ShowEnterDeviceCommand = new RelayCommand(OnShowEnterDevice, CanShowEnterDevice);
			ShowExitDeviceCommand = new RelayCommand(OnShowExitDevice, CanShowExitDevice);

			Devices = new ObservableCollection<DeviceViewModel>();
			Devices.AddRange(DevicesViewModel.Current.AllDevices.FindAll(x => x.Device.UID == door.EnterDeviceUID
				|| x.Device.UID == door.ExitDeviceUID || x.Device.UID == door.LockControlDeviceUID || x.Device.UID == door.LockDeviceUID));
			EnterDevice = Door.EnterDevice;
			ExitDevice = Door.ExitDevice;
		}

		public List<XStateClass> StateClasses
		{
			get
			{
				var stateClasses = State.StateClasses.ToList();
				stateClasses.Sort();
				return stateClasses;
			}
		}

		void OnStateChanged()
		{
			OnPropertyChanged(() => State);
			OnPropertyChanged(() => StateClasses);
		}

		public string PresentationName
		{
			get { return Door.PresentationName; }
		}

		public string PresentationDescription
		{
			get { return Door.Description; }
		}

		public GKDevice EnterDevice { get; private set; }
		public bool HasEnterDevice
		{
			get { return EnterDevice != null; }
		}
		public RelayCommand ShowEnterDeviceCommand { get; private set; }
		void OnShowEnterDevice()
		{
			ServiceFactory.Events.GetEvent<ShowGKDeviceEvent>().Publish(EnterDevice.UID);
		}
		bool CanShowEnterDevice()
		{
			return EnterDevice != null;
		}

		public GKDevice ExitDevice { get; private set; }
		public bool HasExitDevice
		{
			get { return ExitDevice != null; }
		}
		public RelayCommand ShowExitDeviceCommand { get; private set; }
		void OnShowExitDevice()
		{
			ServiceFactory.Events.GetEvent<ShowGKDeviceEvent>().Publish(ExitDevice.UID);
		}
		bool CanShowExitDevice()
		{
			return ExitDevice != null;
		}

		public RelayCommand ShowOnPlanOrPropertiesCommand { get; private set; }
		void OnShowOnPlanOrProperties()
		{
			if (CanShowOnPlan())
				ShowOnPlanHelper.ShowDoor(Door);
			else
				DialogService.ShowWindow(new DoorDetailsViewModel(Door));
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			ShowOnPlanHelper.ShowDoor(Door);
		}
		public bool CanShowOnPlan()
		{
			return ShowOnPlanHelper.CanShowDoor(Door);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			var showArchiveEventArgs = new ShowArchiveEventArgs
			{
				GKDoor = Door
			};
			ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(showArchiveEventArgs);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		private void OnShowProperties()
		{
			DialogService.ShowWindow(new DoorDetailsViewModel(Door));
		}

		public string OpenRegimeLogicName
		{
			get { return GKManager.GetPresentationLogic(Door.OpenRegimeLogic); }
		}

		public string NormRegimeLogicName
		{
			get { return GKManager.GetPresentationLogic(Door.NormRegimeLogic); }
		}

		public string CloseRegimeLogicName
		{
			get { return GKManager.GetPresentationLogic(Door.CloseRegimeLogic); }
		}

		public bool IsBold { get; set; }
	}
}