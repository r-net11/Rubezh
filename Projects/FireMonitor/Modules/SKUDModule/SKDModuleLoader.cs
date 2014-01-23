using System.Collections.Generic;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using SKDModule.ViewModels;
using GKProcessor;
using FiresecClient;
using FiresecAPI.Models.Skud;

namespace SKDModule
{
	public class SKDModuleLoader : ModuleBase
	{
		SKUDViewModel SKUDViewModel;
		NavigationItem _skudNavigationItem;

		public override void CreateViewModels()
		{
			SKUDViewModel = new SKUDViewModel();
		}

		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			_skudNavigationItem = new NavigationItem<ShowSKUDEvent>(SKUDViewModel, "СКД", "/Controls;component/Images/levels.png");
			return new List<NavigationItem>()
		    {
		        _skudNavigationItem
		    };
		}

		public override void Initialize()
		{
			;
		}

		public override string Name
		{
			get { return "СКД"; }
		}
	}
}