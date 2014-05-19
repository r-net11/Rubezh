using System.Collections.Generic;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using AutomationModule.ViewModels;
using AutomationModule.Events;

namespace AutomationModule
{
	public class AutomationModule : ModuleBase
	{
		SoundsViewModel SoundsViewModel;

		public override void CreateViewModels()
		{
			SoundsViewModel = new SoundsViewModel();
		}

		public override void Initialize()
		{
			SoundsViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>
				{
					new NavigationItem("Автоматизация", "/Controls;component/Images/tree.png",
						new List<NavigationItem>()
						{
							new NavigationItem<ShowAutomationSoundsEvent>(SoundsViewModel, "Звуки", "/Controls;component/Images/music.png"),
						}),
				};
		}
		public override string Name
		{
			get { return "Звуки"; }
		}
		public override void RegisterResource()
		{
			base.RegisterResource();
			var resourceService = new ResourceService();
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Sounds/DataTemplates/Dictionary.xaml"));
		}
	}
}