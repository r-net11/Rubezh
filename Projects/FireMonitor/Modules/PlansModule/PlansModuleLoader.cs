using System;
using System.Collections.Generic;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using PlansModule.ViewModels;
using XFiresecAPI;
using Infrustructure.Plans.Events;
using FiresecAPI.Models;
using Infrustructure.Plans;

namespace PlansModule
{
	public class PlansModuleLoader : ModuleBase
	{
		private PlansViewModel _plansViewModel;
		private NavigationItem _planNavigationItem;

		public PlansModuleLoader()
		{
			ServiceFactory.Events.GetEvent<ShowDeviceOnPlanEvent>().Subscribe(OnShowDeviceOnPlan);
			ServiceFactory.Events.GetEvent<ShowZoneOnPlanEvent>().Subscribe(OnShowZoneOnPlan);
			ServiceFactory.Events.GetEvent<ShowXDeviceOnPlanEvent>().Subscribe(OnShowXDeviceOnPlan);
			ServiceFactory.Events.GetEvent<ShowXZoneOnPlanEvent>().Subscribe(OnShowXZoneOnPlan);
			ServiceFactory.Events.GetEvent<RegisterPlanPresenterEvent<Plan>>().Subscribe(OnRegisterPlanPresenter);
			_plansViewModel = new PlansViewModel();
		}

		private void OnShowDeviceOnPlan(Guid deviceUID)
		{
			var hasDeviceOnPlan = _plansViewModel.ShowDevice(deviceUID);
			if (hasDeviceOnPlan)
				ServiceFactory.Events.GetEvent<ShowPlansEvent>().Publish(null);
		}
		private void OnShowZoneOnPlan(Guid zoneUID)
		{
			var hasZoneOnPlan = _plansViewModel.ShowZone(zoneUID);
			if (hasZoneOnPlan)
				ServiceFactory.Events.GetEvent<ShowPlansEvent>().Publish(null);
		}
		private void OnShowXDeviceOnPlan(XDevice device)
		{
			var hasDeviceOnPlan = _plansViewModel.ShowXDevice(device);
			if (hasDeviceOnPlan)
				ServiceFactory.Events.GetEvent<ShowPlansEvent>().Publish(null);
		}
		private void OnShowXZoneOnPlan(XZone zone)
		{
			var hasZoneOnPlan = _plansViewModel.ShowXZone(zone);
			if (hasZoneOnPlan)
				ServiceFactory.Events.GetEvent<ShowPlansEvent>().Publish(null);
		}

		public override void Initialize()
		{
			_planNavigationItem.IsVisible = FiresecManager.PlansConfiguration.Plans.Count > 0;
			_plansViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			_planNavigationItem = new NavigationItem<ShowPlansEvent>(_plansViewModel, "Планы", "/Controls;component/Images/map.png");
			return new List<NavigationItem>() { _planNavigationItem };
		}

		public override string Name
		{
			get { return "Графические планы"; }
		}

		private void OnRegisterPlanPresenter(IPlanPresenter<Plan> planExtension)
		{
			_plansViewModel.RegisterPresenter(planExtension);
		}

	}
}