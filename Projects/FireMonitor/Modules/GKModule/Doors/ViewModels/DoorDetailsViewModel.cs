using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.Events;
using Infrastructure.PlanLink.ViewModels;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace GKModule.ViewModels
{
	public class DoorDetailsViewModel : DialogViewModel, IWindowIdentity
	{
		public GKDoor Door { get; private set; }
		public PlanLinksViewModel Planlink { get; private set; }
		public GKState State
		{
			get { return Door.State; }
		}

		public DoorDetailsViewModel(GKDoor door)
		{
			ShowCommand = new RelayCommand(OnShow);
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			SetAutomaticStateCommand = new RelayCommand(OnSetAutomaticState, CanSetAutomaticState);
			SetManualStateCommand = new RelayCommand(OnSetManualState, CanSetManualState);
			SetIgnoreStateCommand = new RelayCommand(OnSetIgnoreState, CanSetIgnoreState);
			TurnOnCommand = new RelayCommand(OnTurnOn);
			TurnOffCommand = new RelayCommand(OnTurnOff);
			TurnOffNowCommand = new RelayCommand(OnTurnOffNow);
			ResetCommand = new RelayCommand(OnReset);

			SetRegimeNormCommand = new RelayCommand(OnSetRegimeNorm);
			SetRegimeOpenCommand = new RelayCommand(OnSetRegimeOpen);
			SetRegimeCloseCommand = new RelayCommand(OnSetRegimeClose);

			Door = door;
			State.StateChanged -= OnStateChanged;
			State.StateChanged += OnStateChanged;
			Planlink = new PlanLinksViewModel(Door);
			Title = Door.PresentationName;
		}

		void OnStateChanged()
		{
			OnPropertyChanged(() => ControlRegime);
			OnPropertyChanged(() => IsControlRegime);
			OnPropertyChanged(() => State);
			OnPropertyChanged(() => HasOffDelay);
			OnPropertyChanged(() => HasHoldDelay);
			CommandManager.InvalidateRequerySuggested();
		}

		public bool HasOffDelay
		{
			get { return State.StateClasses.Contains(XStateClass.TurningOff) && State.OffDelay > 0; }
		}
		public bool HasHoldDelay
		{
			get { return State.StateClasses.Contains(XStateClass.On) && State.HoldDelay > 0; }
		}

		public DeviceControlRegime ControlRegime
		{
			get
			{
				if (State.StateClasses.Contains(XStateClass.Ignore))
					return DeviceControlRegime.Ignore;

				if (!State.StateClasses.Contains(XStateClass.AutoOff))
					return DeviceControlRegime.Automatic;

				return DeviceControlRegime.Manual;
			}
		}

		public bool IsControlRegime
		{
			get { return ControlRegime == DeviceControlRegime.Manual; }
		}

		public RelayCommand SetAutomaticStateCommand { get; private set; }
		void OnSetAutomaticState()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.FiresecService.GKSetAutomaticRegime(Door);
			}
		}
		bool CanSetAutomaticState()
		{
			return ControlRegime != DeviceControlRegime.Automatic;
		}

		public RelayCommand SetManualStateCommand { get; private set; }
		void OnSetManualState()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.FiresecService.GKSetManualRegime(Door);
			}
		}
		bool CanSetManualState()
		{
			return ControlRegime != DeviceControlRegime.Manual;
		}

		public RelayCommand SetIgnoreStateCommand { get; private set; }
		void OnSetIgnoreState()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.FiresecService.GKSetIgnoreRegime(Door);
			}
		}
		bool CanSetIgnoreState()
		{
			return ControlRegime != DeviceControlRegime.Ignore;
		}

		public RelayCommand TurnOnCommand { get; private set; }
		void OnTurnOn()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.FiresecService.GKTurnOn(Door);
			}
		}

		public RelayCommand TurnOffCommand { get; private set; }
		void OnTurnOff()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.FiresecService.GKTurnOff(Door);
			}
		}

		public RelayCommand TurnOffNowCommand { get; private set; }
		void OnTurnOffNow()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.FiresecService.GKTurnOffNow(Door);
			}
		}

		public RelayCommand ResetCommand { get; private set; }
		void OnReset()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.FiresecService.GKReset(Door);
			}
		}


		public RelayCommand SetRegimeNormCommand { get; private set; }
		void OnSetRegimeNorm()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.FiresecService.GKSetAutomaticRegime(Door);
				ClientManager.FiresecService.GKTurnOffInAutomatic(Door);
			}
		}


		public RelayCommand SetRegimeOpenCommand { get; private set; }
		void OnSetRegimeOpen()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.FiresecService.GKSetManualRegime(Door);
				ClientManager.FiresecService.GKTurnOn(Door);
			}
		}


		public RelayCommand SetRegimeCloseCommand { get; private set; }
		void OnSetRegimeClose()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.FiresecService.GKSetManualRegime(Door);
				ClientManager.FiresecService.GKTurnOffNow(Door);
			}
		}

		public RelayCommand ShowCommand { get; private set; }
		void OnShow()
		{
			ServiceFactory.Events.GetEvent<ShowGKDoorEvent>().Publish(Door.UID);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			if (Door != null)
				ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(new List<Guid> { Door.UID });
		}

		public bool CanControl
		{
			get { return ClientManager.CheckPermission(PermissionType.Oper_Door_Control); }
		}

		public bool FullCanControl
		{
			get { return ClientManager.CheckPermission(PermissionType.Oper_Full_Door_Control); }
		}

		#region IWindowIdentity Members
		public string Guid
		{
			get { return Door.UID.ToString(); }
		}
		#endregion

		public override void OnClosed()
		{
			State.StateChanged -= new Action(OnStateChanged);
		}
	}
}