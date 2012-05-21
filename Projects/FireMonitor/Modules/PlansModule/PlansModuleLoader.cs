using System;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using PlansModule.ViewModels;
using System.Collections.Generic;
using Infrastructure.Common.Navigation;
using FiresecClient;

namespace PlansModule
{
	public class PlansModuleLoader : ModuleBase
	{
		PlansViewModel PlansViewModel;

		public PlansModuleLoader()
		{
			ServiceFactory.Events.GetEvent<ShowPlansEvent>().Subscribe(OnShowPlan);
			ServiceFactory.Events.GetEvent<ShowDeviceOnPlanEvent>().Subscribe(OnShowDeviceOnPlan);
			ServiceFactory.Events.GetEvent<ShowZoneOnPlanEvent>().Subscribe(OnShowZoneOnPlan);

		}

		public void CreateViewModels()
		{
			PlansViewModel = new PlansViewModel();
		}

		void OnShowPlan(object obj)
		{
			ServiceFactory.Layout.Show(PlansViewModel);
		}
		void OnShowDeviceOnPlan(Guid deviceUID)
		{
			ServiceFactory.Layout.Show(PlansViewModel);
			PlansViewModel.ShowDevice(deviceUID);
		}
		void OnShowZoneOnPlan(ulong zoneNo)
		{
			ServiceFactory.Layout.Show(PlansViewModel);
			PlansViewModel.ShowZone(zoneNo);
		}

		public override void Initialize()
		{
			CreateViewModels();
			PlansViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			var navigation = new List<NavigationItem>();
			if (FiresecManager.PlansConfiguration.Plans.Count > 0)
				navigation.Add(new NavigationItem<ShowPlansEvent>("Планы", "/Controls;component/Images/map.png"));
			return navigation;
		}
	}
}