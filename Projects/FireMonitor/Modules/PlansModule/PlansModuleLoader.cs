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

namespace PlansModule
{
	public class PlansModuleLoader : ModuleBase
	{
		private PlansViewModel PlansViewModel;
		private NavigationItem _planNavigationItem;

		public PlansModuleLoader()
		{
			ServiceFactory.Events.GetEvent<ShowDeviceOnPlanEvent>().Subscribe(OnShowDeviceOnPlan);
			ServiceFactory.Events.GetEvent<ShowZoneOnPlanEvent>().Subscribe(OnShowZoneOnPlan);
			ServiceFactory.Events.GetEvent<ShowXDeviceOnPlanEvent>().Subscribe(OnShowXDeviceOnPlan);
			ServiceFactory.Events.GetEvent<ShowXZoneOnPlanEvent>().Subscribe(OnShowXZoneOnPlan);
			PlansViewModel = new PlansViewModel();
		}

		void OnShowDeviceOnPlan(Guid deviceUID)
		{
			var hasDeviceOnPlan = PlansViewModel.ShowDevice(deviceUID);
			if (hasDeviceOnPlan)
				ServiceFactory.Events.GetEvent<ShowPlansEvent>().Publish(null);
		}
        void OnShowZoneOnPlan(Guid zoneUID)
		{
			ServiceFactory.Events.GetEvent<ShowPlansEvent>().Publish(null);
            PlansViewModel.ShowZone(zoneUID);
		}
		void OnShowXDeviceOnPlan(XDevice device)
		{
			var hasDeviceOnPlan = PlansViewModel.ShowXDevice(device);
			if (hasDeviceOnPlan)
				ServiceFactory.Events.GetEvent<ShowPlansEvent>().Publish(null);
		}
		void OnShowXZoneOnPlan(XZone zone)
		{
			var hasZoneOnPlan = PlansViewModel.ShowXZone(zone);
			if (hasZoneOnPlan)
				ServiceFactory.Events.GetEvent<ShowPlansEvent>().Publish(null);
		}

		public override void Initialize()
		{
			_planNavigationItem.IsVisible = FiresecManager.PlansConfiguration.Plans.Count > 0;
			PlansViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			_planNavigationItem = new NavigationItem<ShowPlansEvent>(PlansViewModel, "Планы", "/Controls;component/Images/map.png");
			return new List<NavigationItem>() { _planNavigationItem };
		}

		public override string Name
		{
			get { return "Графические планы"; }
		}
	}
}