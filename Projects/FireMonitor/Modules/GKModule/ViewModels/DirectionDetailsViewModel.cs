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
using Infrustructure.Plans.Elements;
using System.Collections.ObjectModel;

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
			InitializePlans();

			ShowCommand = new RelayCommand(OnShow);
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
				if (DirectionState.StateBits.Contains(XStateBit.Ignore))
					return DeviceControlRegime.Ignore;

				if (DirectionState.StateBits.Contains(XStateBit.Norm))
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
			get { return DirectionState.StateBits.Contains(XStateBit.TurningOn) && DirectionState.OnDelay > 0; }
		}
		public bool HasHoldDelay
		{
			get { return DirectionState.StateBits.Contains(XStateBit.On) && DirectionState.HoldDelay > 0; }
		}

		public RelayCommand ShowCommand { get; private set; }
		void OnShow()
		{
			ServiceFactory.Events.GetEvent<ShowXDirectionEvent>().Publish(Direction.UID);
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
				ElementBase elementBase = plan.ElementRectangleXDirections.FirstOrDefault(x => x.DirectionUID == Direction.UID);
				if (elementBase != null)
				{
					var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
					alarmPlanViewModel.Direction = Direction;
					Plans.Add(alarmPlanViewModel);
					continue;
				}

				elementBase = plan.ElementPolygonXDirections.FirstOrDefault(x => x.DirectionUID == Direction.UID);
				if (elementBase != null)
				{
					var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
					alarmPlanViewModel.Direction = Direction;
					Plans.Add(alarmPlanViewModel);
				}
			}
		}

		#region IWindowIdentity Members
		public string Guid
		{
			get { return Direction.UID.ToString(); }
		}
		#endregion
	}
}