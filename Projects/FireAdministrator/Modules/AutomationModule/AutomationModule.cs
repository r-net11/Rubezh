using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using AutomationModule.Events;
using AutomationModule.Layout.ViewModels;
using AutomationModule.Plans;
using AutomationModule.Validation;
using AutomationModule.ViewModels;
using StrazhAPI;
using StrazhAPI.Enums;
using StrazhAPI.Models;
using StrazhAPI.Models.Layouts;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Layouts;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Validation;
using Infrustructure.Plans.Events;
using Localization.Automation.Common;

namespace AutomationModule
{
	public class AutomationModule : ModuleBase, IValidationModule, ILayoutDeclarationModule
	{
		private ProceduresViewModel _proceduresViewModel;
		private SchedulesViewModel _schedulesViewModel;
		private GlobalVariablesViewModel _globalVariablesViewModel;
		private AutomationPlanExtension _planExtension;

		public override void CreateViewModels()
		{
			_proceduresViewModel = new ProceduresViewModel();
			_schedulesViewModel = new SchedulesViewModel();
			_globalVariablesViewModel = new GlobalVariablesViewModel();
			_planExtension = new AutomationPlanExtension(_proceduresViewModel);
		}

		public override void Initialize()
		{
			ControlVisualStepViewModel.Initialize();
			var automationChanged = ServiceFactory.SaveService.AutomationChanged;
			_proceduresViewModel.Initialize();
			_schedulesViewModel.Initialize();
			_globalVariablesViewModel.Initialize();
			ServiceFactory.SaveService.AutomationChanged = automationChanged;
			_planExtension.Initialize(); //TODO: This is equal to BuildAllSafe() method below;
			ServiceFactoryBase.Events.GetEvent<RegisterPlanExtensionEvent<Plan>>().Publish(_planExtension);
			_planExtension.Cache.BuildAllSafe(); //TODO: Remove
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			// Скрываем в главном меню пункт "Сценарии автоматизации", если лицензия этого требует
			if (!ServiceFactory.UiElementsVisibilityService.IsMainMenuAutomationElementVisible)
				return new List<NavigationItem>();

			return new List<NavigationItem>
				{
					new NavigationItem(ModuleType.ToDescription(), "tree",
						new List<NavigationItem>
						{
                            new NavigationItem<ShowProceduresEvent, Guid>(_proceduresViewModel, CommonResources.Procedure, "Procedure"),
                            new NavigationItem<ShowAutomationSchedulesEvents, Guid>(_schedulesViewModel, CommonResources.Schedules, "Shedules"),
                            new NavigationItem<ShowGlobalVariablesEvent, Guid>(_globalVariablesViewModel, CommonResources.GlobalVariables, "GlobalVariables"),
						}) {IsExpanded = true},
				};
		}

		public override ModuleType ModuleType
		{
			get { return ModuleType.Automation; }
		}

		public override void RegisterResource()
		{
			base.RegisterResource();
			var resourceService = new ResourceService();
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Procedures/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Schedules/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "GlobalVariables/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Layout/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Plans/DataTemplates/Dictionary.xaml"));
		}

		public IEnumerable<IValidationError> Validate()
		{
			var validator = new Validator();
			return validator.Validate();
		}

		#region ILayoutDeclarationModule Members

		public IEnumerable<ILayoutPartDescription> GetLayoutPartDescriptions()
		{
            yield return new LayoutPartDescription(LayoutPartDescriptionGroup.Common, LayoutPartIdentities.AutomationProcedure, 160, CommonResources.Procedure, CommonResources.ExecuteProcedure, "BProcedures.png")
			{
				Factory = p => new LayoutPartProcedureViewModel(p as LayoutPartProcedureProperties),
			};
#if DEBUG
            yield return new LayoutPartDescription(LayoutPartDescriptionGroup.Common, LayoutPartIdentities.Automation, 159, CommonResources.Procedures, CommonResources.ProcedurePanel, "BProcedures.png");
#endif
		}

		#endregion
	}
}