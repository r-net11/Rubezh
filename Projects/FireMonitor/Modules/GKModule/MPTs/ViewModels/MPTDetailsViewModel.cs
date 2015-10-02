﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;

namespace GKModule.ViewModels
{
	public class MPTDetailsViewModel : DialogViewModel, IWindowIdentity
	{
		public GKMPT MPT { get; private set; }
		public GKState State
		{
			get { return MPT.State; }
		}
		public MPTDetailsViewModel(GKMPT mpt)
		{
			MPT = mpt;
			Title = MPT.PresentationName;
			State.StateChanged += new Action(OnStateChanged);
			InitializePlans();

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
				FiresecManager.FiresecService.GKSetAutomaticRegime(MPT);
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
				FiresecManager.FiresecService.GKSetManualRegime(MPT);
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
				FiresecManager.FiresecService.GKSetIgnoreRegime(MPT);
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
				FiresecManager.FiresecService.GKTurnOn(MPT);
			}
		}

		public RelayCommand TurnOnNowCommand { get; private set; }
		void OnTurnOnNow()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				FiresecManager.FiresecService.GKTurnOnNow(MPT);
			}
		}

		public RelayCommand TurnOffCommand { get; private set; }
		void OnTurnOff()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				FiresecManager.FiresecService.GKTurnOff(MPT);
			}
		}

		public RelayCommand ForbidStartCommand { get; private set; }
		void OnForbidStart()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				FiresecManager.FiresecService.GKStop(MPT);
			}
		}

		public bool HasOnDelay
		{
			get { return State.StateClasses.Contains(XStateClass.TurningOn) && State.OnDelay > 0; }
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

		public ObservableCollection<PlanLinkViewModel> Plans { get; private set; }
		public bool HasPlans
		{
			get { return Plans.Count > 0; }
		}

		void InitializePlans()
		{
			Plans = new ObservableCollection<PlanLinkViewModel>();
		}

		public bool CanControl
		{
			get { return  FiresecManager.CheckPermission(PermissionType.Oper_MPT_Control); }
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