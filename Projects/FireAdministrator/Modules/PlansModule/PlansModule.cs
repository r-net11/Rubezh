using System.Collections.Generic;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using PlansModule.ViewModels;

namespace PlansModule
{
	public class PlansModule : ModuleBase
	{
		PlansViewModel _plansViewModel;

		public PlansModule()
		{
			ServiceFactory.Events.GetEvent<ConfigurationSavingEvent>().Subscribe(OnSave);
			ServiceFactory.Events.GetEvent<ShowPlansEvent>().Subscribe(OnShowPlans);
			_plansViewModel = new PlansViewModel();
		}

		void OnShowPlans(object obj)
		{
			ServiceFactory.Layout.Show(_plansViewModel);
		}

		void OnSave(object obj)
		{
			_plansViewModel.PlanDesignerViewModel.Save();
		}

		public override void RegisterResource()
		{
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Designer/Designer/DesignerCanvas.xaml"));
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Designer/Designer/DesignerItem.xaml"));
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Designer/Rectangle/ResizeChromeRectangle.xaml"));
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Designer/Polygon/ResizeChromePolygon.xaml"));
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Designer/Toolbox/Toolbox.xaml"));
		}
		public override void Initialize()
		{
			_plansViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowPlansEvent>("Планы","/Controls;component/Images/map.png"),
			};
		}
		public override string Name
		{
			get { return "Графические планы"; }
		}
	}
}