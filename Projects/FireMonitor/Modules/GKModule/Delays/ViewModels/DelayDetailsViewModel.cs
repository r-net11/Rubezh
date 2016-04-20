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
using System.Linq;
using System.Windows.Input;

namespace GKModule.ViewModels
{
	public class DelayDetailsViewModel : DialogViewModel, IWindowIdentity
	{
		public GKDelay Delay { get; private set; }
		public PlanLinksViewModel PlanLinks { get; private set; }
		public GKState State
		{
			get { return Delay.State; }
		}

		public DelayDetailsViewModel(GKDelay delay)
		{
			Delay = delay;
			Title = Delay.PresentationName;
			State.StateChanged += new Action(OnStateChanged);
			PlanLinks = new PlanLinksViewModel(Delay);

			ShowCommand = new RelayCommand(OnShow);
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			SetAutomaticStateCommand = new RelayCommand(OnSetAutomaticState, CanSetAutomaticState);
			SetManualStateCommand = new RelayCommand(OnSetManualState, CanSetManualState);
			SetIgnoreStateCommand = new RelayCommand(OnSetIgnoreState, CanSetIgnoreState);
			TurnOnCommand = new RelayCommand(OnTurnOn);
			TurnOnNowCommand = new RelayCommand(OnTurnOnNow);
			TurnOffCommand = new RelayCommand(OnTurnOff);
		}

		void OnStateChanged()
		{
			OnPropertyChanged(() => StateClasses);
			OnPropertyChanged(() => ControlRegime);
			OnPropertyChanged(() => IsControlRegime);
			OnPropertyChanged(() => State);
			OnPropertyChanged(() => HasOnDelay);
			OnPropertyChanged(() => HasHoldDelay);
			CommandManager.InvalidateRequerySuggested();
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
				ClientManager.FiresecService.GKSetAutomaticRegime(Delay);
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
				ClientManager.FiresecService.GKSetManualRegime(Delay);
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
				ClientManager.FiresecService.GKSetIgnoreRegime(Delay);
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
				ClientManager.FiresecService.GKTurnOn(Delay);
			}
		}

		public RelayCommand TurnOnNowCommand { get; private set; }
		void OnTurnOnNow()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.FiresecService.GKTurnOnNow(Delay);
			}
		}

		public RelayCommand TurnOffCommand { get; private set; }
		void OnTurnOff()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.FiresecService.GKTurnOff(Delay);
			}
		}

		public bool HasOnDelay
		{
			get { return State.StateClasses.Contains(XStateClass.TurningOn) && State.OnDelay > 0; }
		}
		public bool HasHoldDelay
		{
			get { return State.StateClasses.Contains(XStateClass.On) && State.HoldDelay > 0; }
		}
		public bool HasRunningTime
		{
			get { return State.StateClasses.Contains(XStateClass.On) && State.RunningTime > 0; }
		}

		public RelayCommand ShowCommand { get; private set; }
		void OnShow()
		{
			ServiceFactory.Events.GetEvent<ShowGKDelayEvent>().Publish(Delay.UID);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			if (Delay != null)
				ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(new List<Guid> { Delay.UID });
		}

		public bool CanControl
		{
			get { return ClientManager.CheckPermission(PermissionType.Oper_Delay_Control); }

		}

		#region IWindowIdentity Members
		public string Guid
		{
			get { return Delay.UID.ToString(); }
		}
		#endregion

		public override void OnClosed()
		{
			State.StateChanged -= new Action(OnStateChanged);
		}
	}
}