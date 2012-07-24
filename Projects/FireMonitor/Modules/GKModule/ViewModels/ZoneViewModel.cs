using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class ZoneViewModel : BaseViewModel
	{
		public XZone Zone { get; private set; }
		public XZoneState ZoneState { get; private set; }

		public ZoneViewModel(XZone zone)
		{
			SelectCommand = new RelayCommand(OnSelect);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);

			Zone = zone;
			ZoneState = XManager.DeviceStates.ZoneStates.FirstOrDefault(x => x.No == zone.No);
			ZoneState.StateChanged += new System.Action(OnStateChanged);
			OnStateChanged();
		}

		void OnStateChanged()
		{
			StateType = ZoneState.StateType;
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
				toolTip += "\n" + "Состояние: " + StateType;
				toolTip += "\n" + "Количество датчиков для сработки: " + Zone.DetectorCount.ToString();
				return toolTip;
			}
		}

		public RelayCommand SelectCommand { get; private set; }
		void OnSelect()
		{
			ServiceFactory.Events.GetEvent<ZoneSelectedEvent>().Publish(Zone.No);
		}

		public bool CanShowOnPlan()
		{
			return false;
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				if (plan.ElementPolygonZones.Any(x => (x.ZoneNo.HasValue) && (x.ZoneNo.Value == (ulong)Zone.No)))
				{
					return true;
				}
				if (plan.ElementRectangleZones.Any(x => (x.ZoneNo.HasValue) && (x.ZoneNo.Value == (ulong)Zone.No)))
				{
					return true;
				}
			}
			return false;
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			ServiceFactory.Events.GetEvent<ShowZoneOnPlanEvent>().Publish((ulong)Zone.No);
		}
	}
}