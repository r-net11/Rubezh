using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class ZoneDetailsViewModel : DialogViewModel, IWindowIdentity
	{
		public XZone Zone { get; private set; }
		public XState State
		{
			get { return Zone.State; }
		}

		public ZoneDetailsViewModel(XZone zone)
		{
			ShowCommand = new RelayCommand(OnShow);
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ResetFireCommand = new RelayCommand(OnResetFire, CanResetFire);
			SetIgnoreCommand = new RelayCommand(OnSetIgnore, CanSetIgnore);
			ResetIgnoreCommand = new RelayCommand(OnResetIgnore, CanResetIgnore);

			Zone = zone;
			State.StateChanged += new Action(OnStateChanged);
			InitializePlans();

			Title = Zone.PresentationName;
			TopMost = true;
		}

		void OnStateChanged()
		{
			OnPropertyChanged("State");
			OnPropertyChanged("ResetFireCommand");
			OnPropertyChanged("SetIgnoreCommand");
			OnPropertyChanged("ResetIgnoreCommand");
			CommandManager.InvalidateRequerySuggested();
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
				FiresecManager.FiresecService.GKReset(Zone);
            }
        }
		bool CanResetFire()
		{
			return State.StateClasses.Contains(XStateClass.Fire2) || State.StateClasses.Contains(XStateClass.Fire1) || State.StateClasses.Contains(XStateClass.Attention);
		}

		public RelayCommand SetIgnoreCommand { get; private set; }
        void OnSetIgnore()
        {
            if (ServiceFactory.SecurityService.Validate())
            {
				FiresecManager.FiresecService.GKSetIgnoreRegime(Zone);
            }
        }
		bool CanSetIgnore()
		{
			return !State.StateClasses.Contains(XStateClass.Ignore) && FiresecManager.CheckPermission(PermissionType.Oper_ControlDevices);
		}

		public RelayCommand ResetIgnoreCommand { get; private set; }
        void OnResetIgnore()
        {
            if (ServiceFactory.SecurityService.Validate())
            {
				FiresecManager.FiresecService.GKSetAutomaticRegime(Zone);
            }
        }
		bool CanResetIgnore()
		{
			return State.StateClasses.Contains(XStateClass.Ignore) && FiresecManager.CheckPermission(PermissionType.Oper_ControlDevices);
		}

		public RelayCommand ShowCommand { get; private set; }
		void OnShow()
		{
			ServiceFactory.Events.GetEvent<ShowXZoneEvent>().Publish(Zone.UID);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			var showXArchiveEventArgs = new ShowXArchiveEventArgs()
			{
				Zone = Zone
			};
			ServiceFactory.Events.GetEvent<ShowXArchiveEvent>().Publish(showXArchiveEventArgs);
		}

		public bool CanControl
		{
			get { return FiresecManager.CheckPermission(PermissionType.Oper_ControlDevices); }
		}

		#region IWindowIdentity Members
		public string Guid
		{
			get { return Zone.UID.ToString(); }
		}
		#endregion

		public override void OnClosed()
		{
			State.StateChanged -= new Action(OnStateChanged);
		}
	}
}