using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using Localization.Automation.ViewModels;

namespace AutomationModule.ViewModels
{
	public class StepTypeSelectationViewModel : SaveCancelDialogViewModel
	{
		public StepTypeSelectationViewModel()
		{
			Title = CommonViewModel.StepTypeSelection_Title;

			BuildStepTypeTree();
			FillAllStepTypes();

			RootStepType.IsExpanded = true;
			SelectedStepType = RootStepType;
			foreach (var stepType in AllStepTypes)
			{
				stepType.ExpandToThis();
			}

			OnPropertyChanged(() => RootStepTypes);
		}

		public List<StepTypeViewModel> AllStepTypes;

		public void FillAllStepTypes()
		{
			AllStepTypes = new List<StepTypeViewModel>();
			AddChildPlainStepTypes(RootStepType);
		}

		void AddChildPlainStepTypes(StepTypeViewModel parentViewModel)
		{
			AllStepTypes.Add(parentViewModel);
			foreach (var childViewModel in parentViewModel.Children)
				AddChildPlainStepTypes(childViewModel);
		}

		private StepTypeViewModel _selectedStepType;
		public StepTypeViewModel SelectedStepType
		{
			get { return _selectedStepType; }
			set
			{
				_selectedStepType = value;
				OnPropertyChanged(() => SelectedStepType);
			}
		}

		private StepTypeViewModel _rootStepType;
		public StepTypeViewModel RootStepType
		{
			get { return _rootStepType; }
			private set
			{
				_rootStepType = value;
				OnPropertyChanged(() => RootStepType);
			}
		}

		public StepTypeViewModel[] RootStepTypes
		{
			get { return new[] { RootStepType }; }
		}

		private void BuildStepTypeTree()
		{
			var stepTypeViewModels = new List<StepTypeViewModel>();
			
			var node = BuildOperationsNode();
			if (node.ChildrenCount > 0)
				stepTypeViewModels.Add(node);
			node = BuildInteractiveLogicNode();
			if (node.ChildrenCount > 0)
				stepTypeViewModels.Add(node);
			node = BuildServiceFunctionsNode();
			if (node.ChildrenCount > 0)
				stepTypeViewModels.Add(node);
			node = BuildSeriesFunctionsNode();
			if (node.ChildrenCount > 0)
				stepTypeViewModels.Add(node);
			node = BuildHardwareControlNode();
			if (node.ChildrenCount > 0)
				stepTypeViewModels.Add(node);
			node = BuildDataExchangeNode();
			if (node.ChildrenCount > 0)
				stepTypeViewModels.Add(node);

            RootStepType = new StepTypeViewModel(CommonViewModel.StepTypeSelection_FunctionsRegister, "/Controls;component/Images/CFolder.png", stepTypeViewModels);
		}

		private StepTypeViewModel BuildDataExchangeNode()
		{
			var stepTypeViewModels = new List<StepTypeViewModel>();
			
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.ExportReport))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.ExportReport));
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.ExportJournal))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.ExportJournal));
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.ExportConfiguration))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.ExportConfiguration));
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.ExportOrganisation))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.ExportOrganisation));
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.ExportOrganisationList))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.ExportOrganisationList));
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.ImportOrganisation))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.ImportOrganisation));
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.ImportOrganisationList))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.ImportOrganisationList));

            return new StepTypeViewModel(CommonViewModel.StepTypeSelection_DataExchange, "/Controls;component/StepIcons/Export.png", stepTypeViewModels);
		}

		private StepTypeViewModel BuildOperationsNode()
		{
			var stepTypeViewModels = new List<StepTypeViewModel>();

			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.Arithmetics))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.Arithmetics));
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.SetValue))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.SetValue));
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.IncrementValue))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.IncrementValue));
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.Random))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.Random));
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.GetDateTimeNow))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.GetDateTimeNow));
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.GenerateGuid))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.GenerateGuid));

            return new StepTypeViewModel(CommonViewModel.StepTypeSelection_Operations, "/Controls;component/StepIcons/Arithmetics.png", stepTypeViewModels);
		}

		private StepTypeViewModel BuildInteractiveLogicNode()
		{
			var stepTypeViewModels = new List<StepTypeViewModel>();

			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.PlaySound))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.PlaySound));
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.AddJournalItem))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.AddJournalItem));
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.SendEmail))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.SendEmail));
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.ShowMessage))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.ShowMessage));
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.ControlVisualGet))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.ControlVisualGet));
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.ControlVisualSet))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.ControlVisualSet));
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.ControlPlanGet))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.ControlPlanGet));
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.ControlPlanSet))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.ControlPlanSet));
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.ShowDialog))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.ShowDialog));

            return new StepTypeViewModel(CommonViewModel.StepTypeSelection_Logic, "/Controls;component/StepIcons/PlaySound.png", stepTypeViewModels);
		}

		private StepTypeViewModel BuildServiceFunctionsNode()
		{
			var stepTypeViewModels = new List<StepTypeViewModel>();

			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.Exit))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.Exit));
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.RunProgram))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.RunProgram));
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.Pause))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.Pause));
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.ProcedureSelection))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.ProcedureSelection));
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.CheckPermission))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.CheckPermission));
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.GetJournalItem))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.GetJournalItem));
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.GetSkdDeviceProperty))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.GetSkdDeviceProperty));
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.GetDoorProperty))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.GetDoorProperty));
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.GetSkdZoneProperty))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.GetSkdZoneProperty));

            return new StepTypeViewModel(CommonViewModel.StepTypeSelection_ServiceFunctions, "/Controls;component/StepIcons/Exit.png", stepTypeViewModels);
		}

		private StepTypeViewModel BuildSeriesFunctionsNode()
		{
			var stepTypeViewModels = new List<StepTypeViewModel>();

			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.For))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.For));
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.While))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.While));
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.Break))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.Break));
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.Continue))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.Continue));

            return new StepTypeViewModel(CommonViewModel.StepTypeSelection_CycleFunctions, "/Controls;component/StepIcons/For.png", stepTypeViewModels);
		}

		private StepTypeViewModel BuildHardwareControlNode()
		{
			var stepTypeViewModels = new List<StepTypeViewModel>();

			var node = BuildStrazhControlNode();
			if (node.ChildrenCount > 0)
				stepTypeViewModels.Add(node);
			node = BuildVideoControlNode();
			if (node.ChildrenCount > 0)
				stepTypeViewModels.Add(node);

            return new StepTypeViewModel(CommonViewModel.StepTypeSelection_HardwareControl, "/Controls;component/StepIcons/Control.png", stepTypeViewModels);
		}

		private StepTypeViewModel BuildStrazhControlNode()
		{
			var stepTypeViewModels = new List<StepTypeViewModel>();

			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.ControlSKDDevice))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.ControlSKDDevice));
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.ControlSKDZone))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.ControlSKDZone));
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.ControlDoor))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.ControlDoor));

            return new StepTypeViewModel(CommonViewModel.StepTypeSelection_StrazhControl, "/Controls;component/StepIcons/Control.png", stepTypeViewModels);
		}

		private StepTypeViewModel BuildVideoControlNode()
		{
			var stepTypeViewModels = new List<StepTypeViewModel>();

			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.StartRecord))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.StartRecord));
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.Ptz))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.Ptz));
			if (ServiceFactory.UiElementsVisibilityService.VisibleProcedureSteps.Any(x => x == ProcedureStepType.RviAlarm))
				stepTypeViewModels.Add(new StepTypeViewModel(ProcedureStepType.RviAlarm));

            return new StepTypeViewModel(CommonViewModel.StepTypeSelection_VideoControl, "/Controls;component/StepIcons/Control.png", stepTypeViewModels);
		}

		protected override bool CanSave()
		{
			return ((SelectedStepType != null) && (!SelectedStepType.IsFolder));
		}
	}
}