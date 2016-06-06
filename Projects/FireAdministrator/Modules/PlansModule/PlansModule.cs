using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Layouts;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Validation;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Designer;
using Infrastructure.Events;
using Infrustructure.Plans;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Painters;
using PlansModule.ViewModels;
using StrazhAPI.Enums;
using StrazhAPI.Models;
using StrazhAPI.Models.Layouts;
using System.Collections.Generic;

namespace PlansModule
{
	public class PlansModule : ModuleBase, ILayoutDeclarationModule, IValidationModule
	{
		private PlansViewModel _plansViewModel;

		public override int Order
		{
			get { return 100; }
		}
		public override void CreateViewModels()
		{
			ServiceFactoryBase.Events.GetEvent<ConfigurationClosedEvent>().Subscribe(OnConfigurationClosedEvent);
			ServiceFactoryBase.Events.GetEvent<RegisterPlanExtensionEvent<Plan>>().Subscribe(OnRegisterPlanExtension);
			ServiceFactoryBase.Events.GetEvent<ConfigurationSavingEvent>().Subscribe(OnConfigurationSavingEvent);
			_plansViewModel = new PlansViewModel();
			ApplicationService.Starting += (s, e) => ShowRightContent();
		}

		public override void RegisterResource()
		{
			base.RegisterResource();
			DesignerLoader.RegisterResource();
		}
		public override void Initialize()
		{
			_plansViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>
			{
#if PLAN_TAB
				new NavigationItem<ShowPlansEvent>(PlansViewModel, "Планы","map"),
#endif
			};
		}
		public override ModuleType ModuleType
		{
			get { return ModuleType.Plans; }
		}
		private void ShowRightContent()
		{
#if !PLAN_TAB
			var viewModel = new RightContentViewModel
			{
				Content = _plansViewModel,
				Menu = _plansViewModel.Menu,
			};
			ServiceFactory.Layout.ShowRightContent(viewModel);
#endif
		}

		private void OnRegisterPlanExtension(IPlanExtension<Plan> planExtension)
		{
			_plansViewModel.RegisterExtension(planExtension);
		}
		private void OnConfigurationSavingEvent(object obj)
		{
			_plansViewModel.PlanDesignerViewModel.Save();
		}
		private static void OnConfigurationClosedEvent(object obj)
		{
			PainterCache.Dispose();
		}

		#region ILayoutDeclarationModule Members
		public IEnumerable<ILayoutPartDescription> GetLayoutPartDescriptions()
		{
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.Common, LayoutPartIdentities.Plans, 150, "Планы", "Планы", "CMap.png")
			{
				Factory = p => new LayoutPartPlansViewModel(p as LayoutPartPlansProperties),
			};
		}
		#endregion

		#region IValidationModule Members

		public IEnumerable<IValidationError> Validate()
		{
			FiresecManager.PlansConfiguration.Update();
			return _plansViewModel.Validate();
		}

		#endregion
	}
#if PLAN_TAB
	public class ShowPlansEvent : CompositePresentationEvent<object>
	{
	}
#endif
}