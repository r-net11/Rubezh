using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using System.Collections.Generic;
using System;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class DoorViewModel : BaseViewModel
	{
		public GKDevice EnterDevice { get; private set; }
		public GKDevice ExitDevice { get; private set; }
		public GKDevice EnterButton { get; private set; }
		public GKDevice ExitButton { get; private set; }
		public GKDevice LockDevice { get; private set; }
		public GKDevice LockDeviceExit { get; private set; }
		public GKDevice LockControlDevice { get; private set; }
		public GKDevice LockControlDeviceExit { get; private set; }
		public GKSKDZone EnterZone { get; private set; }
		public GKSKDZone ExitZone { get; private set; }
		public GKDoor Door { get; private set; }
		public GKState State
		{
			get { return Door.State; }
		}
		public DoorDetailsViewModel DoorDetailsViewModel { get; private set; }

		public DoorViewModel(GKDoor door)
		{
			Door = door;
			DoorDetailsViewModel = new DoorDetailsViewModel(door);
			State.StateChanged -= OnStateChanged;
			State.StateChanged += OnStateChanged;
			OnStateChanged();

			EnterDevice = GKManager.Devices.FirstOrDefault(x => x.UID == Door.EnterDeviceUID);
			ExitDevice = GKManager.Devices.FirstOrDefault(x => x.UID == Door.ExitDeviceUID);
			EnterButton = GKManager.Devices.FirstOrDefault(x => x.UID == Door.EnterButtonUID);
			ExitButton = GKManager.Devices.FirstOrDefault(x => x.UID == Door.ExitButtonUID);
			LockDevice = GKManager.Devices.FirstOrDefault(x => x.UID == Door.LockDeviceUID);
			LockDeviceExit = GKManager.Devices.FirstOrDefault(x => x.UID == Door.LockDeviceExitUID);
			LockControlDevice = GKManager.Devices.FirstOrDefault(x => x.UID == Door.LockControlDeviceUID);
			LockControlDeviceExit = GKManager.Devices.FirstOrDefault(x => x.UID == Door.LockControlDeviceExitUID);
			EnterZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == Door.EnterZoneUID);
			ExitZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == Door.ExitZoneUID);

			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			//ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			ShowOnPlanOrPropertiesCommand = new RelayCommand(OnShowOnPlanOrProperties);
			ShowDeviceCommand = new RelayCommand<GKDevice>(OnShowDevice);
			ShowZoneCommand = new RelayCommand<GKSKDZone>(OnShowZone);
		}

		void OnStateChanged()
		{
			OnPropertyChanged(() => State);
			OnPropertyChanged(() => State.StateClasses);
		}

		public RelayCommand ShowOnPlanOrPropertiesCommand { get; private set; }
		void OnShowOnPlanOrProperties()
		{
			var plan = ShowOnPlanHelper.GetPlan(Door.UID);
			if (plan!= null)
				ShowOnPlanHelper.ShowObjectOnPlan(plan, Door.UID);
			else
				DialogService.ShowWindow(new DoorDetailsViewModel(Door));
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			ShowOnPlanHelper.ShowObjectOnPlan(ShowOnPlanHelper.GetPlan(Door.UID), Door.UID );
		}
		public bool CanShowOnPlan()
		{
			return ShowOnPlanHelper.GetPlan(Door.UID)!= null;
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			if (Door != null)
				ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(new List<Guid> { Door.UID });
		}

		//public RelayCommand ShowPropertiesCommand { get; private set; }
		//private void OnShowProperties()
		//{
		//	DialogService.ShowWindow(new DoorDetailsViewModel(Door));
		//}

		public RelayCommand<GKDevice> ShowDeviceCommand { get; private set; }
		private void OnShowDevice(GKDevice device)
		{
			if (device != null)
				ServiceFactory.Events.GetEvent<ShowGKDeviceEvent>().Publish(device.UID);
		}

		public RelayCommand<GKSKDZone> ShowZoneCommand { get; private set; }
		private void OnShowZone(GKSKDZone zone)
		{
			if (zone != null)
				ServiceFactory.Events.GetEvent<ShowGKSKDZoneEvent>().Publish(zone.UID);
		}

		public string OpenRegimeLogicName
		{
			get { return GKManager.GetPresentationLogic(Door.OpenRegimeLogic.OnClausesGroup); }
		}

		public string NormRegimeLogicName
		{
			get { return GKManager.GetPresentationLogic(Door.NormRegimeLogic.OnClausesGroup); }
		}

		public string CloseRegimeLogicName
		{
			get { return GKManager.GetPresentationLogic(Door.CloseRegimeLogic.OnClausesGroup); }
		}

		public bool IsBold { get; set; }
	}
}