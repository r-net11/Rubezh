using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class ZoneDetailsViewModel : DialogViewModel, IWindowIdentity
	{
		public SKDZone Zone { get; private set; }
		public SKDZoneState State
		{
			get { return Zone.State; }
		}

		public ZoneDetailsViewModel(SKDZone zone)
		{
			ShowCommand = new RelayCommand(OnShow);
			OpenAllCommand = new RelayCommand(OnOpenAll, CanOpenAll);
			CloseAllCommand = new RelayCommand(OnCloseAll, CanCloseAll);
			DetectEmployeesCommand = new RelayCommand(OnDetectEmployees, CanDetectEmployees);

			Zone = zone;
			State.StateChanged += new Action(OnStateChanged);
			InitializePlans();

			Title = Zone.Name;
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
				elementBase = plan.ElementRectangleSKDZones.FirstOrDefault(x => x.ZoneUID == Zone.UID);
				if (elementBase != null)
				{
					var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
					alarmPlanViewModel.Zone = Zone;
					Plans.Add(alarmPlanViewModel);
					continue;
				}

				elementBase = plan.ElementPolygonSKDZones.FirstOrDefault(x => x.ZoneUID == Zone.UID);
				if (elementBase != null)
				{
					var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
					alarmPlanViewModel.Zone = Zone;
					Plans.Add(alarmPlanViewModel);
				}
			}
		}

		public RelayCommand OpenAllCommand { get; private set; }
		void OnOpenAll()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				//FiresecManager.FiresecService.SKDReset(Zone);
			}
		}
		bool CanOpenAll()
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_ControlDevices);
		}

		public RelayCommand CloseAllCommand { get; private set; }
		void OnCloseAll()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				//FiresecManager.FiresecService.SKDReset(Zone);
			}
		}
		bool CanCloseAll()
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_ControlDevices);
		}

		public RelayCommand DetectEmployeesCommand { get; private set; }
		void OnDetectEmployees()
		{
		}
		bool CanDetectEmployees()
		{
			return true;
		}

		public RelayCommand ShowCommand { get; private set; }
		void OnShow()
		{
			ServiceFactory.Events.GetEvent<ShowSKDZoneEvent>().Publish(Zone.UID);
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