using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using PlansModule.Events;
using XFiresecAPI;
using Infrustructure.Plans;

namespace PlansModule.ViewModels
{
	public class PlansViewModel : ViewPartViewModel
	{
		private List<IPlanPresenter<Plan>> _planPresenters;
		public PlanTreeViewModel PlanTreeViewModel { get; private set; }
		public PlanDesignerViewModel PlanDesignerViewModel { get; private set; }

		public PlansViewModel()
		{
			ServiceFactory.Events.GetEvent<SelectPlanEvent>().Subscribe(OnSelectPlan);
			_planPresenters = new List<IPlanPresenter<Plan>>();
			PlanTreeViewModel = new PlanTreeViewModel();
			PlanDesignerViewModel = new PlanDesignerViewModel(_planPresenters);
			PlanTreeViewModel.SelectedPlanChanged += (s, e) => OnSelectedPlanChanged();
		}

		public void Initialize()
		{
			FiresecManager.InvalidatePlans();
			_planPresenters.Clear();
			PlanTreeViewModel.Initialize();
			OnSelectedPlanChanged();
		}

		public PlanViewModel SelectedPlan
		{
			get { return PlanTreeViewModel.SelectedPlan; }
			set { PlanTreeViewModel.SelectedPlan = value; }
		}
		private void OnSelectedPlanChanged()
		{
			if (SelectedPlan != null)
				PlanDesignerViewModel.Initialize(SelectedPlan);
			OnPropertyChanged("SelectedPlan");
		}
		public void OnSelectPlan(Guid planUID)
		{
			SelectedPlan = PlanTreeViewModel.Plans.FirstOrDefault(x => x.Plan.UID == planUID);
		}

		public void RegisterPresenter(IPlanPresenter<Plan> planExtension)
		{
			if (!_planPresenters.Contains(planExtension))
				_planPresenters.Add(planExtension);
		}

		public bool ShowDevice(Guid deviceUID)
		{
			foreach (var planViewModel in PlanTreeViewModel.Plans)
			{
				if (planViewModel.DeviceStates.Any(x => x.Device.UID == deviceUID))
				{
					SelectedPlan = planViewModel;
					PlanDesignerViewModel.SelectDevice(deviceUID);
					return true;
				}
			}
			return false;
		}
		public bool ShowXDevice(XDevice device)
		{
			foreach (var planViewModel in PlanTreeViewModel.Plans)
			{
				if (planViewModel.XDeviceStates.Any(x => x.Device == device))
				{
					SelectedPlan = planViewModel;
					PlanDesignerViewModel.SelectXDevice(device);
					return true;
				}
			}
			return false;
		}
		public bool ShowXZone(XZone zone)
		{
			foreach (var planViewModel in PlanTreeViewModel.Plans)
			{
				if (planViewModel.XZoneStates.Any(x => x.Zone == zone))
				{
					SelectedPlan = planViewModel;
					PlanDesignerViewModel.SelectXZone(zone);
					return true;
				}
			}
			return false;
		}
		public bool ShowZone(Guid zoneUID)
		{
			foreach (var planViewModel in PlanTreeViewModel.Plans)
			{
				foreach (var zone in planViewModel.Plan.ElementPolygonZones.Where(x => x.ZoneUID != Guid.Empty))
				{
					if (zone.ZoneUID == zoneUID)
					{
						SelectedPlan = planViewModel;
						PlanDesignerViewModel.SelectZone(zoneUID);
						return true;
					}
				}
				foreach (var zone in planViewModel.Plan.ElementRectangleZones.Where(x => x.ZoneUID != Guid.Empty))
				{
					if (zone.ZoneUID == zoneUID)
					{
						SelectedPlan = planViewModel;
						PlanDesignerViewModel.SelectZone(zoneUID);
						return true;
					}
				}
			}
			return false;
		}
	}
}