using System.Collections.Generic;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Events;
using Infrustructure.Plans;
using Infrustructure.Plans.Painters;
using PlansModule.ViewModels;

namespace PlansModule
{
	public class PlansModuleLoader : ModuleBase, ILayoutProviderModule
	{
		private PlansViewModel _plansViewModel;
		private NavigationItem _planNavigationItem;

		public override void CreateViewModels()
		{
            PainterCache.UseTransparentImage = false;
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
			FiresecManager.UpdatePlansConfiguration();
			_planNavigationItem.IsVisible = FiresecManager.PlansConfiguration.Plans.Count > 0;
			_plansViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			_planNavigationItem = new NavigationItem<ShowPlansEvent>(_plansViewModel, "Планы", "/Controls;component/Images/map.png");
			return new List<NavigationItem>() { _planNavigationItem };
		}

		#region ILayoutProviderModule Members
		public IEnumerable<ILayoutPartPresenter> GetLayoutParts()
		{
			yield return new LayoutPartPresenter(LayoutPartIdentities.Plans, "Планы", "Map.png", (p) => _plansViewModel);
		}
		#endregion
	}
}