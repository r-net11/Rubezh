using System.Collections.Generic;
using FiresecAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class StepTypeSelectationViewModel : SaveCancelDialogViewModel
	{
		public StepTypeSelectationViewModel()
		{
			Title = "Выбор типа функции";

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

		StepTypeViewModel _selectedStepType;
		public StepTypeViewModel SelectedStepType
		{
			get { return _selectedStepType; }
			set
			{
				_selectedStepType = value;
				OnPropertyChanged(() => SelectedStepType);
			}
		}

		StepTypeViewModel _rootStepType;
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
			get { return new StepTypeViewModel[] { RootStepType }; }
		}

		void BuildStepTypeTree()
		{
			RootStepType = new StepTypeViewModel("Реестр функций",
				new List<StepTypeViewModel>()
				{
					new StepTypeViewModel("Интерактивные функции",
						new List<StepTypeViewModel>()
						{
							new StepTypeViewModel(ProcedureStepType.ProcedureSelection),
						}),
					new StepTypeViewModel("Поля объектов",
						new List<StepTypeViewModel>()
						{
							new StepTypeViewModel(ProcedureStepType.SetObjectField),
							new StepTypeViewModel(ProcedureStepType.GetObjectField),
						}),
					new StepTypeViewModel("Проигрывание звука",
						new List<StepTypeViewModel>()
						{
							new StepTypeViewModel(ProcedureStepType.PlaySound),
						}),
					new StepTypeViewModel("Различная логика",
						new List<StepTypeViewModel>()
						{
							new StepTypeViewModel(ProcedureStepType.Arithmetics),
							new StepTypeViewModel(ProcedureStepType.PersonInspection),
							new StepTypeViewModel(ProcedureStepType.FindObjects),
							new StepTypeViewModel(ProcedureStepType.SendMessage),
							new StepTypeViewModel(ProcedureStepType.ReportExport),
						}),
					new StepTypeViewModel("Служебные функции",
						new List<StepTypeViewModel>()
						{
							new StepTypeViewModel(ProcedureStepType.Exit),
							new StepTypeViewModel(ProcedureStepType.SetGlobalValue),
							new StepTypeViewModel(ProcedureStepType.RunProgramm),
							new StepTypeViewModel(ProcedureStepType.IncrementGlobalValue),
							new StepTypeViewModel(ProcedureStepType.SendEmail),
							new StepTypeViewModel(ProcedureStepType.Pause),
							new StepTypeViewModel(ProcedureStepType.SendDebugMessage),
						}),
					new StepTypeViewModel("Управление аппаратурой",
						new List<StepTypeViewModel>()
						{
							new StepTypeViewModel("Управление ГК",
								new List<StepTypeViewModel>()
								{
									new StepTypeViewModel(ProcedureStepType.ControlGKDevice),
									new StepTypeViewModel(ProcedureStepType.ControlGKFireZone),
									new StepTypeViewModel(ProcedureStepType.ControlGKGuardZone),
								}),
							new StepTypeViewModel("Управление СКД",
								new List<StepTypeViewModel>()
								{
									new StepTypeViewModel(ProcedureStepType.ControlSKDGKDevice),
									new StepTypeViewModel(ProcedureStepType.ControlSKDZone),
								}),
							new StepTypeViewModel("Управление Видео",
								new List<StepTypeViewModel>()
								{
									new StepTypeViewModel(ProcedureStepType.ControlCamera),
								}),
						}),

				});
		}

		protected override bool CanSave()
		{
			return ((SelectedStepType != null)&&(!SelectedStepType.IsFolder));
		}
	}
}