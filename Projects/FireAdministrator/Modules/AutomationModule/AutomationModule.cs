using System;
using System.Collections.Generic;
using AutomationModule.Events;
using AutomationModule.Layout.ViewModels;
using AutomationModule.Plans;
using AutomationModule.Validation;
using AutomationModule.ViewModels;
using FiresecAPI;
using FiresecAPI.Automation;
using FiresecAPI.Models;
using FiresecAPI.Models.Layouts;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Validation;
using Infrustructure.Plans.Events;
using FiresecClient;

namespace AutomationModule
{
	public class AutomationModule : ModuleBase, IValidationModule, ILayoutDeclarationModule
	{
		SoundsViewModel SoundsViewModel;
		ProceduresViewModel ProceduresViewModel;
		SchedulesViewModel SchedulesViewModel;
		GlobalVariablesViewModel GlobalVariablesViewModel;
		AutomationPlanExtension _planExtension;

		public override void CreateViewModels()
		{
			//FiresecManager.SystemConfiguration.AutomationConfiguration = new AutomationConfiguration();
			SoundsViewModel = new SoundsViewModel();
			ProceduresViewModel = new ProceduresViewModel();
			SchedulesViewModel = new SchedulesViewModel();
			GlobalVariablesViewModel = new GlobalVariablesViewModel();
			_planExtension = new AutomationPlanExtension(ProceduresViewModel);
		}

		public override void Initialize()
		{
			var automationChanged = ServiceFactory.SaveService.AutomationChanged;
			SoundsViewModel.Initialize();
			ProceduresViewModel.Initialize();
			SchedulesViewModel.Initialize();
			GlobalVariablesViewModel.Initialize();
			ServiceFactory.SaveService.AutomationChanged = automationChanged;
			_planExtension.Initialize();
			ServiceFactory.Events.GetEvent<RegisterPlanExtensionEvent<Plan>>().Publish(_planExtension);
			_planExtension.Cache.BuildAllSafe();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>
				{
					new NavigationItem(ModuleType.ToDescription(), "/Controls;component/Images/tree.png",
						new List<NavigationItem>()
						{
							new NavigationItem<ShowProceduresEvent, Guid>(ProceduresViewModel, "Процедуры", "/Controls;component/Images/Procedure.png"),
							new NavigationItem<ShowAutomationSchedulesEvents, Guid>(SchedulesViewModel, "Расписания", "/Controls;component/Images/Shedules.png"),
							new NavigationItem<ShowGlobalVariablesEvent, Guid>(GlobalVariablesViewModel, "Глобальные переменные", "/Controls;component/Images/GlobalVariables.png"),
							new NavigationItem<ShowAutomationSoundsEvent, Guid>(SoundsViewModel, "Звуки", "/Controls;component/Images/Music.png")
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
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Sounds/DataTemplates/Dictionary.xaml"));
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
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.Common, LayoutPartIdentities.AutomationProcedure, 160, "Процедура", "Выпонить процедуру", "BProcedures.png")
			{
				Factory = (p) => new LayoutPartProcedureViewModel(p as LayoutPartProcedureProperties),
			};
		}

		#endregion
	}
}