using System;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using GKModule.Events;

namespace GKModule.ViewModels
{
	public class DoorViewModel : BaseViewModel
	{
		public GKDoor Door { get; private set; }
		public GKState State
		{
			get { return Door.State; }
		}

		public DoorViewModel(GKDoor door)
		{
			Door = door;
			State.StateChanged -= new Action(OnStateChanged);
			State.StateChanged += new Action(OnStateChanged);
			OnStateChanged();

			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			ShowOnPlanOrPropertiesCommand = new RelayCommand(OnShowOnPlanOrProperties);
			ShowEnterDeviceCommand = new RelayCommand(OnShowEnterDevice, CanShowEnterDevice);
			ShowExitDeviceCommand = new RelayCommand(OnShowExitDevice, CanShowExitDevice);

			EnterDevice = Door.EnterDevice;
			ExitDevice = Door.ExitDevice;
		}

		void OnStateChanged()
		{
			OnPropertyChanged(() => State);
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
			var showArchiveEventArgs = new ShowArchiveEventArgs()
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

		public bool IsBold { get; set; }
	}
}