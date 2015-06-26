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
using Infrustructure.Plans.Elements;

namespace GKModule.ViewModels
{
	public class DirectionDetailsViewModel : DialogViewModel, IWindowIdentity
	{
		public GKDirection Direction { get; private set; }
		public GKState State
		{
			get { return Direction.State; }
		}

		public DirectionDetailsViewModel(GKDirection direction)
		{
			Direction = direction;
			Title = Direction.PresentationName;
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

		public int InputZonesCount
		{
			get { return Direction.InputZones.Count; }
		}
		public int InputDevicesCount
		{
			get { return Direction.InputDevices.Count; }
		}
		public int OutputDevicesCount
		{
			get { return Direction.OutputDevices.Count; }
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
				FiresecManager.FiresecService.GKSetAutomaticRegime(Direction);
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
				FiresecManager.FiresecService.GKSetManualRegime(Direction);
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
				FiresecManager.FiresecService.GKSetIgnoreRegime(Direction);
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
				FiresecManager.FiresecService.GKTurnOn(Direction);
			}
		}

		public RelayCommand TurnOnNowCommand { get; private set; }
		void OnTurnOnNow()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				FiresecManager.FiresecService.GKTurnOnNow(Direction);
			}
		}

		public RelayCommand TurnOffCommand { get; private set; }
		void OnTurnOff()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				FiresecManager.FiresecService.GKTurnOff(Direction);
			}
		}

		public RelayCommand ForbidStartCommand { get; private set; }
		void OnForbidStart()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				FiresecManager.FiresecService.GKStop(Direction);
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
			ServiceFactory.Events.GetEvent<ShowGKDirectionEvent>().Publish(Direction.UID);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			var showArchiveEventArgs = new ShowArchiveEventArgs()
			{
				GKDirection = Direction
			};
			ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(showArchiveEventArgs);
		}

		public ObservableCollection<PlanLinkViewModel> Plans { get; private set; }
		public bool HasPlans
		{
			get { return Plans.Count > 0; }
		}

		void InitializePlans()
		{
			Plans = new ObservableCollection<PlanLinkViewModel>();
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				ElementBase elementBase = plan.ElementRectangleGKDirections.FirstOrDefault(x => x.DirectionUID == Direction.UID);
				if (elementBase != null)
				{
					var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
					alarmPlanViewModel.Direction = Direction;
					Plans.Add(alarmPlanViewModel);
					continue;
				}

				elementBase = plan.ElementPolygonGKDirections.FirstOrDefault(x => x.DirectionUID == Direction.UID);
				if (elementBase != null)
				{
					var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
					alarmPlanViewModel.Direction = Direction;
					Plans.Add(alarmPlanViewModel);
				}
			}
		}

		public bool CanControl
		{
			get { return FiresecManager.CheckPermission(PermissionType.Oper_Directions_Control); }
		}

		#region IWindowIdentity Members
		public string Guid
		{
			get { return Direction.UID.ToString(); }
		}
		#endregion

		public override void OnClosed()
		{
			State.StateChanged -= new Action(OnStateChanged);
		}
	}
}