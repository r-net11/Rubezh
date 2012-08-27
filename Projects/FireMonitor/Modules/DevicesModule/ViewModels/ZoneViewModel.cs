using System.Linq;
using Common;
using DevicesModule.Events;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using FiresecAPI;

namespace DevicesModule.ViewModels
{
	public class ZoneViewModel : BaseViewModel
	{
		public ZoneState ZoneState { get; private set; }
		public Zone Zone
		{
			get { return ZoneState.Zone; }
		}

		public ZoneViewModel(ZoneState zoneState)
		{
			SelectCommand = new RelayCommand(OnSelect);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			SetGuardCommand = new RelayCommand(OnSetGuard, CanSetGuard);
			UnSetGuardCommand = new RelayCommand(OnUnSetGuard, CanUnSetGuard);

			ZoneState = zoneState;
			if (FiresecManager.DeviceStates == null)
			{
				Logger.Error("ZoneViewModel.ctrl FiresecManager.DeviceStates = null");
				return;
			}
			if (FiresecManager.DeviceStates.ZoneStates == null)
			{
				Logger.Error("ZoneViewModel.ctrl FiresecManager.DeviceStates.ZoneStates = null");
				return;
			}
			ZoneState.StateChanged += new System.Action(OnStateChanged);
			OnStateChanged();
		}

		void OnStateChanged()
		{
			StateType = ZoneState.StateType;
			OnPropertyChanged("ZoneState");
			OnPropertyChanged("Tooltip");
		}

		StateType _stateType;
		public StateType StateType
		{
			get { return _stateType; }
			set
			{
				_stateType = value;
				OnPropertyChanged("StateType");
			}
		}

		public string Tooltip
		{
			get
			{
				var toolTip = Zone.PresentationName;
				toolTip += "\n" + "Состояние: " + EnumsConverter.StateTypeToClassName(StateType);
				if (Zone.ZoneType == ZoneType.Fire)
				{
					toolTip += "\n" + "Количество датчиков для сработки: " + Zone.DetectorCount.ToString();
				}
				if (Zone.ZoneType == ZoneType.Guard)
				{
					if (FiresecManager.IsZoneOnGuardAlarm(ZoneState))
						toolTip += "\n" + "Охранная тревога";
					else
					{
						if (FiresecManager.IsZoneOnGuard(ZoneState))
							toolTip += "\n" + "На охране";
						else
							toolTip += "\n" + "Не на охране";
					}
				}
				return toolTip;
			}
		}

		public RelayCommand SelectCommand { get; private set; }
		void OnSelect()
		{
			ServiceFactory.Events.GetEvent<ZoneSelectedEvent>().Publish(Zone.No);
		}

		bool CanShowOnPlan()
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				if (plan.ElementPolygonZones.Any(x => (x.ZoneNo.HasValue) && (x.ZoneNo.Value == Zone.No)))
				{
					return true;
				}
				if (plan.ElementRectangleZones.Any(x => (x.ZoneNo.HasValue) && (x.ZoneNo.Value == Zone.No)))
				{
					return true;
				}
			}
			return false;
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			ServiceFactory.Events.GetEvent<ShowZoneOnPlanEvent>().Publish(Zone.No);
		}

		public RelayCommand SetGuardCommand { get; private set; }
		void OnSetGuard()
		{
			if (ServiceFactory.SecurityService.Validate())
				FiresecManager.SetZoneGuard(Zone);
		}
		bool CanSetGuard()
		{
			return ((Zone.ZoneType == ZoneType.Guard) && (Zone.SecPanelUID != null) && (FiresecManager.IsZoneOnGuard(ZoneState) == false));
		}

		public RelayCommand UnSetGuardCommand { get; private set; }
		void OnUnSetGuard()
		{
			if (ServiceFactory.SecurityService.Validate())
				FiresecManager.UnSetZoneGuard(Zone);
		}
		bool CanUnSetGuard()
		{
			return ((Zone.ZoneType == ZoneType.Guard) && (Zone.SecPanelUID != null) && (FiresecManager.IsZoneOnGuard(ZoneState) == true));
		}
	}
}