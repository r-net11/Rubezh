using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;

namespace GKModule.ViewModels
{
	public class SKDZoneDetailsViewModel : DialogViewModel, IWindowIdentity
	{
		public GKSKDZone Zone { get; private set; }
		public GKState State
		{
			get { return Zone.State; }
		}
		public SKDZoneDetailsViewModel(GKSKDZone zone)
		{
			ShowCommand = new RelayCommand(OnShow);
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			OpenCommand = new RelayCommand(OnOpen);
			CloseCommand = new RelayCommand(OnClose);

			Zone = zone;
			Title = Zone.PresentationName;
			State.StateChanged += new Action(OnStateChanged);
			InitializePlans();
		}

		void OnStateChanged()
		{
			OnPropertyChanged(() => StateClasses);
			OnPropertyChanged(() => State);
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

		public ObservableCollection<PlanLinkViewModel> Plans { get; private set; }
		public bool HasPlans
		{
			get { return Plans.Count > 0; }
		}

		void InitializePlans()
		{
			Plans = new ObservableCollection<PlanLinkViewModel>();
			foreach (var plan in ClientManager.PlansConfiguration.AllPlans)
			{
				ElementBase elementBase;
				elementBase = plan.ElementRectangleGKSKDZones.FirstOrDefault(x => x.ZoneUID == Zone.UID);
				if (elementBase != null)
				{
					var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
					alarmPlanViewModel.GkBaseEntity = Zone;
					Plans.Add(alarmPlanViewModel);
					continue;
				}

				elementBase = plan.ElementPolygonGKSKDZones.FirstOrDefault(x => x.ZoneUID == Zone.UID);
				if (elementBase != null)
				{
					var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
					alarmPlanViewModel.GkBaseEntity = Zone;
					Plans.Add(alarmPlanViewModel);
				}
			}
		}

		public RelayCommand OpenCommand { get; private set; }
		void OnOpen()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.FiresecService.GKOpenSKDZone(Zone);
			}
		}

		public RelayCommand CloseCommand { get; private set; }
		void OnClose()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.FiresecService.GKCloseSKDZone(Zone);
			}
		}

		public RelayCommand ShowCommand { get; private set; }
		void OnShow()
		{
			ServiceFactory.Events.GetEvent<ShowGKSKDZoneEvent>().Publish(Zone.UID);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			if (Zone != null)
				ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(new List<Guid> { Zone.UID });
		}
		public bool CanControl
		{
			get { return ClientManager.CheckPermission(PermissionType.Oper_ZonesSKD); }
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