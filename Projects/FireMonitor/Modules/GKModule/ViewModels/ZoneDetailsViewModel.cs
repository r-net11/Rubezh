using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using FiresecClient;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure;
using Infrastructure.Events;

namespace GKModule.ViewModels
{
	public class ZoneDetailsViewModel : DialogViewModel, IWindowIdentity
	{
		Guid _guid;
		public XZone Zone { get; private set; }
		public XZoneState ZoneState { get; private set; }

		public ZoneDetailsViewModel(XZone zone)
		{
			ShowCommand = new RelayCommand(OnShow);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ResetFireCommand = new RelayCommand(OnResetFire, CanResetFire);
			SetIgnoreCommand = new RelayCommand(OnSetIgnore, CanSetIgnore);
			ResetIgnoreCommand = new RelayCommand(OnResetIgnore, CanResetIgnore);

			_guid = zone.UID;
			Zone = zone;
			ZoneState = Zone.ZoneState;
			ZoneState.StateChanged += new Action(OnStateChanged);

			Title = Zone.PresentationName;
			TopMost = true;
		}

		void OnStateChanged()
		{
			var stateClass = ZoneState.StateClass;
			OnPropertyChanged("ZoneState");
			OnPropertyChanged("ResetFireCommand");
			OnPropertyChanged("SetIgnoreCommand");
			OnPropertyChanged("ResetIgnoreCommand");
		}

		public string PlanName
		{
			get
			{
				foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
				{
					if (plan.ElementRectangleXZones.Any(x => x.ZoneUID == Zone.UID))
					{
						return plan.Caption;
					}
					if (plan.ElementPolygonXZones.Any(x => x.ZoneUID == Zone.UID))
					{
						return plan.Caption;
					}
				}
				return null;
			}
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			ShowOnPlanHelper.ShowZone(Zone);
		}
		public bool CanShowOnPlan()
		{
			return ShowOnPlanHelper.CanShowZone(Zone);
		}

		public RelayCommand ResetFireCommand { get; private set; }
		void OnResetFire()
		{
			ObjectCommandSendHelper.ResetZone(Zone);
		}
		bool CanResetFire()
		{
			return ZoneState.States.Contains(XStateType.Fire2) || ZoneState.States.Contains(XStateType.Fire1) || ZoneState.States.Contains(XStateType.Attention);
		}

		public RelayCommand SetIgnoreCommand { get; private set; }
		void OnSetIgnore()
		{
			ObjectCommandSendHelper.SetIgnoreRegimeForZone(Zone);
		}
		bool CanSetIgnore()
		{
			return !ZoneState.States.Contains(XStateType.Ignore) && FiresecManager.CheckPermission(PermissionType.Oper_AddToIgnoreList);
		}

		public RelayCommand ResetIgnoreCommand { get; private set; }
		void OnResetIgnore()
		{
			ObjectCommandSendHelper.SetAutomaticRegimeForZone(Zone);
		}
		bool CanResetIgnore()
		{
			return ZoneState.States.Contains(XStateType.Ignore) && FiresecManager.CheckPermission(PermissionType.Oper_AddToIgnoreList);
		}

		public RelayCommand ShowCommand { get; private set; }
		void OnShow()
		{
			ServiceFactory.Events.GetEvent<ShowXZoneEvent>().Publish(Zone.UID);
		}

		#region IWindowIdentity Members
		public string Guid
		{
			get { return _guid.ToString(); }
		}
		#endregion
	}
}