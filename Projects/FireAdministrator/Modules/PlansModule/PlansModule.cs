using System.Collections.Generic;
using FiresecAPI.Models;
using FiresecAPI.Models.Layouts;
using Infrastructure;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Designer;
using Infrastructure.Events;
using Infrustructure.Plans;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Painters;
using PlansModule.ViewModels;

namespace PlansModule
{
	public class PlansModule : ModuleBase, ILayoutDeclarationModule
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
			base.RegisterResource();
			DesignerLoader.RegisterResource();
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
		protected override ModuleType ModuleType
		{
			get { return ModuleType.Plans; }
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

		#region ILayoutDeclarationModule Members
		public IEnumerable<ILayoutPartDescription> GetLayoutPartDescriptions()
		{
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.Common, LayoutPartIdentities.Plans, 150, "Планы", "Планы", "CMap.png")
			{
				Factory = (p) => new LayoutPartPlansViewModel(p as LayoutPartPlansProperties),
			};
		}
		#endregion
	}
}