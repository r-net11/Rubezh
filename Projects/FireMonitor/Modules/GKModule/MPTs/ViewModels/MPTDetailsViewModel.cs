using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Interfaces;
using Infrastructure.PlanLink.ViewModels;

namespace GKModule.ViewModels
{
	public class MPTDetailsViewModel : DialogViewModel, IWindowIdentity
	{
		public GKMPT MPT { get; private set; }
		public PlanLinksViewModel PlanLinks { get; private set; }
		public GKState State
		{
			get { return MPT.State; }
		}
		public MPTDetailsViewModel(GKMPT mpt)
		{
			MPT = mpt;
			Title = MPT.PresentationName;
			State.StateChanged += new Action(OnStateChanged);
			PlanLinks = new PlanLinksViewModel(MPT);

			ShowCommand = new RelayCommand(OnShow);
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			SetAutomaticStateCommand = new RelayCommand(OnSetAutomaticState, CanSetAutomaticState);
			SetManualStateCommand = new RelayCommand(OnSetManualState, CanSetManualState);
			SetIgnoreStateCommand = new RelayCommand(OnSetIgnoreState, CanSetIgnoreState);
			TurnOnCommand = new RelayCommand(OnTurnOn);
			TurnOnNowCommand = new RelayCommand(OnTurnOnNow);
			TurnOffCommand = new RelayCommand(OnTurnOff);
			ForbidStartCommand = new RelayCommand(OnForbidStart);
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
				ClientManager.FiresecService.GKSetAutomaticRegime(MPT);
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
				ClientManager.FiresecService.GKSetManualRegime(MPT);
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
				ClientManager.FiresecService.GKSetIgnoreRegime(MPT);
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
				ClientManager.FiresecService.GKTurnOn(MPT);
			}
		}

		public RelayCommand TurnOnNowCommand { get; private set; }
		void OnTurnOnNow()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.FiresecService.GKTurnOnNow(MPT);
			}
		}

		public RelayCommand TurnOffCommand { get; private set; }
		void OnTurnOff()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.FiresecService.GKTurnOff(MPT);
			}
		}

		public RelayCommand ForbidStartCommand { get; private set; }
		void OnForbidStart()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.FiresecService.GKStop(MPT);
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

		public RelayCommand ShowCommand { get; private set; }
		void OnShow()
		{
			ServiceFactory.Events.GetEvent<ShowGKMPTEvent>().Publish(MPT.UID);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			if (MPT != null)
				ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(new List<Guid> { MPT.UID });
		}

		public bool CanControl
		{
			get { return  ClientManager.CheckPermission(PermissionType.Oper_MPT_Control); }
		}

		#region IWindowIdentity Members
		public string Guid
		{
			get { return MPT.UID.ToString(); }
		}
		#endregion

		public override void OnClosed()
		{
			State.StateChanged -= new Action(OnStateChanged);
		}
	}
}