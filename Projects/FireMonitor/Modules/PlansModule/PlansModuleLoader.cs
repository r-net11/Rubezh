using System.Collections.Generic;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using Infrustructure.Plans;
using PlansModule.ViewModels;

namespace PlansModule
{
	public class PlansModuleLoader : ModuleBase
	{
		private PlansViewModel _plansViewModel;
		private NavigationItem _planNavigationItem;

		public override void CreateViewModels()
		{
			EventService.RegisterEventAggregator(ServiceFactory.Events);
			_plansViewModel = new PlansViewModel();
		}

		public override int Order
		{
			get { return 100; }
		}
		public override string Name
		{
			get { return "Графические планы"; }
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
	}
}