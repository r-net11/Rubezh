using System;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System.Diagnostics;
using Infrastructure;
using Infrastructure.Events;
using FiresecClient;

namespace GKModule.ViewModels
{
	public class DirectionDetailsViewModel : DialogViewModel, IWindowIdentity
	{
		public XDirection Direction { get; private set; }
		public XDirectionState DirectionState { get; private set; }
		public DirectionViewModel DirectionViewModel { get; private set; }

		public DirectionDetailsViewModel(XDirection direction)
		{
			Direction = direction;
			DirectionState = Direction.DirectionState;
			DirectionViewModel = new DirectionViewModel(DirectionState);
			DirectionState.StateChanged += new Action(OnStateChanged);

			ShowCommand = new RelayCommand(OnShow);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			SetAutomaticStateCommand = new RelayCommand(OnSetAutomaticState);
			SetManualStateCommand = new RelayCommand(OnSetManualState);
			SetIgnoreStateCommand = new RelayCommand(OnSetIgnoreState);
			TurnOnCommand = new RelayCommand(OnTurnOn);
			TurnOnNowCommand = new RelayCommand(OnTurnOnNow);
			TurnOffCommand = new RelayCommand(OnTurnOff);

			Title = Direction.PresentationName;
			TopMost = true;
		}

		void OnStateChanged()
		{
			OnPropertyChanged("ControlRegime");
			OnPropertyChanged("IsControlRegime");
			OnPropertyChanged("DirectionState");
			OnPropertyChanged("HasOnDelay");
			OnPropertyChanged("HasHoldDelay");
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
				if (DirectionState.States.Contains(XStateType.Ignore))
					return DeviceControlRegime.Ignore;

				if (DirectionState.States.Contains(XStateType.Norm))
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
			ObjectCommandSendHelper.SetAutomaticRegimeForDirection(Direction);
		}

		public RelayCommand SetManualStateCommand { get; private set; }
		void OnSetManualState()
		{
			ObjectCommandSendHelper.SetManualRegimeForDirection(Direction);
		}

		public RelayCommand SetIgnoreStateCommand { get; private set; }
		void OnSetIgnoreState()
		{
			ObjectCommandSendHelper.SetIgnoreRegimeForDirection(Direction);
		}

		public RelayCommand TurnOnCommand { get; private set; }
		void OnTurnOn()
		{
			ObjectCommandSendHelper.TurnOnDirection(Direction);
		}

		public RelayCommand TurnOnNowCommand { get; private set; }
		void OnTurnOnNow()
		{
			ObjectCommandSendHelper.TurnOnNowDirection(Direction);
		}

		public RelayCommand TurnOffCommand { get; private set; }
		void OnTurnOff()
		{
			ObjectCommandSendHelper.TurnOffDirection(Direction);
		}

		public bool HasOnDelay
		{
			get { return DirectionState.States.Contains(XStateType.TurningOn) && DirectionState.OnDelay > 0; }
		}
		public bool HasHoldDelay
		{
			get { return DirectionState.States.Contains(XStateType.On) && DirectionState.HoldDelay > 0; }
		}

		public RelayCommand ShowCommand { get; private set; }
		void OnShow()
		{
			ServiceFactory.Events.GetEvent<ShowXDirectionEvent>().Publish(Direction.UID);
		}

		public string PlanName
		{
			get
			{
				foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
				{
					if (plan.ElementRectangleXDirections.Any(x => x.DirectionUID == Direction.UID))
						return plan.Caption;
					if (plan.ElementPolygonXDirections.Any(x => x.DirectionUID == Direction.UID))
						return plan.Caption;
				}
				return null;
			}
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			ShowOnPlanHelper.ShowDirection(Direction);
		}
		public bool CanShowOnPlan()
		{
			return ShowOnPlanHelper.CanShowDirection(Direction);
		}

		#region IWindowIdentity Members
		public string Guid
		{
			get { return Direction.UID.ToString(); }
		}
		#endregion
	}
}