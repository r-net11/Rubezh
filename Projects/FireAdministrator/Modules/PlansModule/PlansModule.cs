using System.Collections.Generic;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using Infrustructure.Plans;
using Infrustructure.Plans.Events;
using PlansModule.ViewModels;

namespace PlansModule
{
	public class PlansModule : ModuleBase
	{
		PlansViewModel PlansViewModel;

		public override int Order
		{
			get { return 100; }
		}
		public override void CreateViewModels()
		{
			ServiceFactory.Events.GetEvent<RegisterPlanExtensionEvent<Plan>>().Subscribe(OnRegisterPlanExtension);
			ServiceFactory.Events.GetEvent<ConfigurationSavingEvent>().Subscribe(OnSave);
			PlansViewModel = new PlansViewModel();
		}

		void OnSave(object obj)
		{
			PlansViewModel.PlanDesignerViewModel.Save();
		}

		public override void RegisterResource()
		{
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Designer/Designer/MoveThumb.xaml"));
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Designer/DesignerItems/DesignerItem.xaml"));
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Designer/Adorners/Adorners.xaml"));
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Designer/Resize/ResizeThumb.xaml"));
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Designer/Resize/DesignerItemBorder.xaml"));
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Designer/Toolbox/Toolbox.xaml"));
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Designer/InstrumentAdorners/RemoveButton.xaml"));
		}
		public override void Initialize()
		{
			PlansViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowPlansEvent>(PlansViewModel, "Планы","/Controls;component/Images/map.png"),
			};
		}
		public override string Name
		{
			get { return "Графические планы"; }
		}

		private void OnRegisterPlanExtension(IPlanExtension<Plan> planExtension)
		{
			PlansViewModel.RegisterExtension(planExtension);
		}
	}
}