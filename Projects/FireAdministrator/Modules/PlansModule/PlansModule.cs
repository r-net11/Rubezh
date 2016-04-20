using Infrastructure;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Validation;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Designer;
using Infrastructure.Designer.Events;
using Infrastructure.Events;
using Infrastructure.Plans;
using Infrastructure.Plans.Events;
using Infrastructure.Plans.Painters;
using PlansModule.ViewModels;
using RubezhAPI.Models;
using RubezhAPI.Models.Layouts;
using RubezhClient;
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
			ServiceFactory.Events.GetEvent<ConfigurationClosedEvent>().Subscribe(OnConfigurationClosedEvent);
			ServiceFactory.Events.GetEvent<RegisterPlanExtensionEvent<Plan>>().Subscribe(OnRegisterPlanExtension);
			ServiceFactory.Events.GetEvent<ConfigurationSavingEvent>().Subscribe(OnConfigurationSavingEvent);
			ServiceFactory.Events.GetEvent<EditPlanElementBindingEvent>().Subscribe(OnEditPlanElementBindingEvent);
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
			return new List<NavigationItem>();
		}
		public override ModuleType ModuleType
		{
			get { return ModuleType.Plans; }
		}
		private void ShowRightContent()
		{
			var viewModel = new RightContentViewModel()
			{
				Content = _plansViewModel,
				Menu = _plansViewModel.Menu,
			};
			ServiceFactory.Layout.ShowRightContent(viewModel);
		}

		private void OnRegisterPlanExtension(IPlanExtension<Plan> planExtension)
		{
			_plansViewModel.RegisterExtension(planExtension);
		}
		private void OnConfigurationSavingEvent(object obj)
		{
			_plansViewModel.PlanDesignerViewModel.Save();
		}

		void OnEditPlanElementBindingEvent(EditPlanElementBindingEventArgs editPlanElementBindingEventArgs)
		{
			var planPropertyBindingViewModel = new PlanPropertyBindingViewModel(editPlanElementBindingEventArgs.PlanElementBindingItem);
			ServiceFactory.DialogService.ShowModalWindow(planPropertyBindingViewModel);
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

		#region IValidationModule Members

		public IEnumerable<IValidationError> Validate()
		{
			ClientManager.PlansConfiguration.Update();
			return _plansViewModel.Validate();
		}

		#endregion
	}
}