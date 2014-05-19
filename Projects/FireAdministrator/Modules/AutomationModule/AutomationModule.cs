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
		ProceduresViewModel ProceduresViewModel;

		public override void CreateViewModels()
		{
			SoundsViewModel = new SoundsViewModel();
			ProceduresViewModel = new ProceduresViewModel();
		}

		public override void Initialize()
		{
			SoundsViewModel.Initialize();
			ProceduresViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>
				{
					new NavigationItem("Автоматизация", "/Controls;component/Images/tree.png",
						new List<NavigationItem>()
						{
							new NavigationItem<ShowAutomationSoundsEvent>(SoundsViewModel, "Звуки", "/Controls;component/Images/Music.png"),
							new NavigationItem<ShowProceduresEvent>(ProceduresViewModel, "Процедуры", "/Controls;component/Images/Tree.png"),
						}),
				};
		}
		public override string Name
		{
			get { return "Автоматизация"; }
		}
		public override void RegisterResource()
		{
			base.RegisterResource();
			var resourceService = new ResourceService();
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Sounds/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Procedures/DataTemplates/Dictionary.xaml"));
		}
	}
}