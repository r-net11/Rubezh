using System.Collections.Generic;
using System.Linq;
using Common.GK;
using FiresecAPI;
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
		public XZoneState ZoneState { get; private set; }
		public XZone Zone
		{
			get { return ZoneState.Zone; }
		}

		public ZoneViewModel(XZoneState zoneState)
		{
			SelectCommand = new RelayCommand(OnSelect);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ResetFire1Command = new RelayCommand(OnResetFire1, CanResetFire1);
			ResetFire2Command = new RelayCommand(OnResetFire2, CanResetFire2);
			SetIgnoreCommand = new RelayCommand(OnSetIgnore, CanSetIgnore);
			ResetIgnoreCommand = new RelayCommand(OnResetIgnore, CanResetIgnore);

			ZoneState = zoneState;
			ZoneState.StateChanged += new System.Action(OnStateChanged);
			OnStateChanged();
		}

		void OnStateChanged()
		{
			OnPropertyChanged("ZoneState");
			OnPropertyChanged("ToolTip");
		}

		public string ToolTip
		{
			get
			{
				var toolTip = Zone.PresentationName;
				toolTip += "\n" + "Состояние: " + ZoneState.StateType.ToDescription();
				toolTip += "\n" + "Количество датчиков для перевода в Пожар 1: " + Zone.Fire1Count.ToString();
				toolTip += "\n" + "Количество датчиков для перевода в Пожар 2: " + Zone.Fire2Count.ToString();
				return toolTip;
			}
		}

		public RelayCommand SelectCommand { get; private set; }
		void OnSelect()
		{
			ServiceFactory.Events.GetEvent<ZoneSelectedEvent>().Publish(Zone.UID);
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			ServiceFactory.Events.GetEvent<ShowZoneOnPlanEvent>().Publish((int)Zone.No);
		}
		public bool CanShowOnPlan()
		{
			return false;
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				if (plan.ElementPolygonZones.Any(x => (x.ZoneNo.HasValue) && (x.ZoneNo.Value == (int)Zone.No)))
				{
					return true;
				}
				if (plan.ElementRectangleZones.Any(x => (x.ZoneNo.HasValue) && (x.ZoneNo.Value == (int)Zone.No)))
				{
					return true;
				}
			}
			return false;
		}

		public RelayCommand ResetFire1Command { get; private set; }
		void OnResetFire1()
		{
			SendControlCommand(0x02);
		}
		bool CanResetFire1()
		{
			return ZoneState.States.Contains(XStateType.Fire1);
		}

		public RelayCommand ResetFire2Command { get; private set; }
		void OnResetFire2()
		{
			SendControlCommand(0x02);
		}
		bool CanResetFire2()
		{
			return ZoneState.States.Contains(XStateType.Fire2);
		}

		public RelayCommand SetIgnoreCommand { get; private set; }
		void OnSetIgnore()
		{
			SendControlCommand(0x86);
		}
		bool CanSetIgnore()
		{
			return !ZoneState.States.Contains(XStateType.Ignore);
		}

		public RelayCommand ResetIgnoreCommand { get; private set; }
		void OnResetIgnore()
		{
			SendControlCommand(0x06);
		}
		bool CanResetIgnore()
		{
			return ZoneState.States.Contains(XStateType.Ignore);
		}

		void SendControlCommand(byte code)
		{
			var bytes = new List<byte>();
			bytes.AddRange(BytesHelper.ShortToBytes(Zone.GetDatabaseNo(DatabaseType.Gk)));
			bytes.Add(code);
			SendManager.Send(Zone.GkDatabaseParent, 3, 13, 0, bytes);
		}
	}
}