using System.Collections.Generic;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Painters;
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
			ServiceFactory.Events.GetEvent<ConfigurationClosedEvent>().Subscribe(OnConfigurationClosedEvent);
			ServiceFactory.Events.GetEvent<RegisterPlanExtensionEvent<Plan>>().Subscribe(OnRegisterPlanExtension);
			ServiceFactory.Events.GetEvent<ConfigurationSavingEvent>().Subscribe(OnConfigurationSavingEvent);
			PlansViewModel = new PlansViewModel();
			ApplicationService.Starting += (s, e) => ShowRightContent();
		}

		public override void RegisterResource()
		{
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Designer/Toolbox/Toolbox.xaml"));
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Designer/InstrumentAdorners/GridLineShape.xaml"));
		}
		public override void Initialize()
		{
			PlansViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				//new NavigationItem<ShowPlansEvent>(PlansViewModel, "Планы","/Controls;component/Images/map.png"),
			};
		}
		public override string Name
		{
			get { return "Графические планы"; }
		}
		private void ShowRightContent()
		{
			var viewModel = new RightContentViewModel()
			{
				Content = PlansViewModel,
				Menu = PlansViewModel.Menu,
			};
			ServiceFactory.Layout.ShowRightContent(viewModel);
		}

		private void OnRegisterPlanExtension(IPlanExtension<Plan> planExtension)
		{
			PlansViewModel.RegisterExtension(planExtension);
		}
		private void OnConfigurationSavingEvent(object obj)
		{
			PlansViewModel.PlanDesignerViewModel.Save();
		}
		private void OnConfigurationClosedEvent(object obj)
		{
			PainterCache.Dispose();
		}
	}
}