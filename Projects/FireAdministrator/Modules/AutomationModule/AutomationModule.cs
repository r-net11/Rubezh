using System;
using System.Collections.Generic;
using AutomationModule.Events;
using AutomationModule.Layout.ViewModels;
using AutomationModule.Plans;
using AutomationModule.Validation;
using AutomationModule.ViewModels;
using RubezhAPI;
using RubezhAPI.Models;
using RubezhAPI.Models.Layouts;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Validation;
using Infrustructure.Plans.Events;

namespace AutomationModule
{
	public class AutomationModule : ModuleBase, IValidationModule, ILayoutDeclarationModule
	{
		private SoundsViewModel _soundsViewModel;
		private OPCServersViewModel _opcServersViewModel;
		private ProceduresViewModel _proceduresViewModel;
		private SchedulesViewModel _schedulesViewModel;
		private GlobalVariablesViewModel _globalVariablesViewModel;
		private AutomationPlanExtension _planExtension;

		public override void CreateViewModels()
		{
			_soundsViewModel = new SoundsViewModel();
			_opcServersViewModel = new OPCServersViewModel();
			_proceduresViewModel = new ProceduresViewModel();
			_schedulesViewModel = new SchedulesViewModel();
			_globalVariablesViewModel = new GlobalVariablesViewModel();
			_planExtension = new AutomationPlanExtension(_proceduresViewModel);
		}

		public override void Initialize()
		{
			ControlVisualStepViewModel.Initialize();
			var automationChanged = ServiceFactory.SaveService.AutomationChanged;
			_soundsViewModel.Initialize();
			_opcServersViewModel.Initialize();
			_proceduresViewModel.Initialize();
			_schedulesViewModel.Initialize();
			_globalVariablesViewModel.Initialize();
			ServiceFactory.SaveService.AutomationChanged = automationChanged;
			_planExtension.Initialize();
			ServiceFactory.Events.GetEvent<RegisterPlanExtensionEvent<Plan>>().Publish(_planExtension);
			_planExtension.Cache.BuildAllSafe();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>
				{
					new NavigationItem(ModuleType.ToDescription(), "tree",
						new List<NavigationItem>()
						{
							new NavigationItem<ShowProceduresEvent, Guid>(_proceduresViewModel, "Процедуры", "Procedure"),
							new NavigationItem<ShowAutomationSchedulesEvents, Guid>(_schedulesViewModel, "Расписания", "Shedules"),
							new NavigationItem<ShowGlobalVariablesEvent, Guid>(_globalVariablesViewModel, "Глобальные переменные", "GlobalVariables"),
							new NavigationItem<ShowAutomationSoundsEvent, Guid>(_soundsViewModel, "Звуки", "Music"),
							new NavigationItem<ShowOPCServersEvent, Guid>(_opcServersViewModel, "OPC Сервера", "Settings2")
						}) { IsExpanded = true },
				};
		}

		public override ModuleType ModuleType
		{
			get { return ModuleType.Automation; }
		}

		public override void RegisterResource()
		{
			base.RegisterResource();
			ServiceFactory.ResourceService.AddResource(GetType().Assembly, "Sounds/DataTemplates/Dictionary.xaml");
			ServiceFactory.ResourceService.AddResource(GetType().Assembly, "OPCServers/DataTemplates/Dictionary.xaml");
			ServiceFactory.ResourceService.AddResource(GetType().Assembly, "Procedures/DataTemplates/Dictionary.xaml");
			ServiceFactory.ResourceService.AddResource(GetType().Assembly, "Schedules/DataTemplates/Dictionary.xaml");
			ServiceFactory.ResourceService.AddResource(GetType().Assembly, "GlobalVariables/DataTemplates/Dictionary.xaml");
			ServiceFactory.ResourceService.AddResource(GetType().Assembly, "Layout/DataTemplates/Dictionary.xaml");
			ServiceFactory.ResourceService.AddResource(GetType().Assembly, "Plans/DataTemplates/Dictionary.xaml");
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
#if DEBUG
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.Common, LayoutPartIdentities.Automation, 159, "Процедуры", "Панель процедуры", "BProcedures.png");
#endif
		}

		#endregion
	}
}