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
		JournalViewModel JournalViewModel;
		NavigationItem _skudNavigationItem;

		public override void CreateViewModels()
		{
			SKUDViewModel = new SKUDViewModel();
			JournalViewModel = new JournalViewModel();
		}

		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			_skudNavigationItem = new NavigationItem<ShowSKUDEvent>(SKUDViewModel, "СКД", "/Controls;component/Images/levels.png");
			return new List<NavigationItem>
				{
				new NavigationItem("СКД", "/Controls;component/Images/tree.png",
					new List<NavigationItem>()
					{
						_skudNavigationItem,
						new NavigationItem<ShowSKDJournalEvent>(JournalViewModel, "Журнал", "/Controls;component/Images/levels.png")
					})
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
		public override void RegisterResource()
		{
			base.RegisterResource();
			var resourceService = new ResourceService();
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Journal/DataTemplates/Dictionary.xaml"));
		}
	}
}