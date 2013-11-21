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
using System.Collections.ObjectModel;
using Infrustructure.Plans.Elements;

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
			ResetFireCommand = new RelayCommand(OnResetFire, CanResetFire);
			SetIgnoreCommand = new RelayCommand(OnSetIgnore, CanSetIgnore);
			ResetIgnoreCommand = new RelayCommand(OnResetIgnore, CanResetIgnore);

			_guid = zone.UID;
			Zone = zone;
			ZoneState = Zone.ZoneState;
			ZoneState.StateChanged += new Action(OnStateChanged);
			InitializePlans();

			Title = Zone.PresentationName;
			TopMost = true;
		}

		void OnStateChanged()
		{
			OnPropertyChanged("ZoneState");
			OnPropertyChanged("ResetFireCommand");
			OnPropertyChanged("SetIgnoreCommand");
			OnPropertyChanged("ResetIgnoreCommand");
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
				ElementBase elementBase;
				elementBase = plan.ElementRectangleXZones.FirstOrDefault(x => x.ZoneUID == Zone.UID);
				if (elementBase != null)
				{
					var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
					alarmPlanViewModel.Zone = Zone;
					Plans.Add(alarmPlanViewModel);
					continue;
				}

				elementBase = plan.ElementPolygonXZones.FirstOrDefault(x => x.ZoneUID == Zone.UID);
				if (elementBase != null)
				{
					var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
					alarmPlanViewModel.Zone = Zone;
					Plans.Add(alarmPlanViewModel);
				}
			}
		}

		public RelayCommand ResetFireCommand { get; private set; }
        void OnResetFire()
        {
            if (ServiceFactory.SecurityService.Validate())
            {
                ObjectCommandSendHelper.Reset(Zone);
            }
        }
		bool CanResetFire()
		{
			return ZoneState.StateClasses.Contains(XStateClass.Fire2) || ZoneState.StateClasses.Contains(XStateClass.Fire1) || ZoneState.StateClasses.Contains(XStateClass.Attention);
		}

		public RelayCommand SetIgnoreCommand { get; private set; }
        void OnSetIgnore()
        {
            if (ServiceFactory.SecurityService.Validate())
            {
                ObjectCommandSendHelper.SetIgnoreRegime(Zone);
            }
        }
		bool CanSetIgnore()
		{
			return !ZoneState.StateClasses.Contains(XStateClass.Ignore) && FiresecManager.CheckPermission(PermissionType.Oper_AddToIgnoreList);
		}

		public RelayCommand ResetIgnoreCommand { get; private set; }
        void OnResetIgnore()
        {
            if (ServiceFactory.SecurityService.Validate())
            {
                ObjectCommandSendHelper.SetAutomaticRegime(Zone);
            }
        }
		bool CanResetIgnore()
		{
			return ZoneState.StateClasses.Contains(XStateClass.Ignore) && FiresecManager.CheckPermission(PermissionType.Oper_AddToIgnoreList);
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

		public override void OnClosed()
		{
			ZoneState.StateChanged -= new Action(OnStateChanged);
		}
	}
}