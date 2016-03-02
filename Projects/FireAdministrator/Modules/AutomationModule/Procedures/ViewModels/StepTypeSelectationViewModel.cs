using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Automation;
using System.Collections.Generic;

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
			get { return new[] { RootStepType }; }
		}

		void BuildStepTypeTree()
		{
			RootStepType = new StepTypeViewModel("Реестр функций", "/Controls;component/Images/CFolder.png",
				new List<StepTypeViewModel>
				{
					new StepTypeViewModel("Операции", "/Controls;component/StepIcons/Arithmetics.png",
						new List<StepTypeViewModel>
						{
							new StepTypeViewModel(ProcedureStepType.Arithmetics),
							new StepTypeViewModel(ProcedureStepType.SetValue),
							new StepTypeViewModel(ProcedureStepType.IncrementValue),
							new StepTypeViewModel(ProcedureStepType.Now),
							new StepTypeViewModel(ProcedureStepType.FindObjects),
							new StepTypeViewModel(ProcedureStepType.GetObjectProperty),
							new StepTypeViewModel(ProcedureStepType.Random),
							new StepTypeViewModel(ProcedureStepType.GenerateGuid),
							new StepTypeViewModel(ProcedureStepType.SetJournalItemGuid)
						}),
						new StepTypeViewModel("Функции управления списками", "/Controls;component/StepIcons/ChangeList.png",
							new List<StepTypeViewModel>
						{
							new StepTypeViewModel(ProcedureStepType.ChangeList),
							new StepTypeViewModel(ProcedureStepType.GetListCount),
							new StepTypeViewModel(ProcedureStepType.GetListItem)
						}),
						new StepTypeViewModel("Интерактивная логика", "/Controls;component/StepIcons/PlaySound.png",
						new List<StepTypeViewModel>
						{
							new StepTypeViewModel(ProcedureStepType.PlaySound),
							new StepTypeViewModel(ProcedureStepType.AddJournalItem),
							new StepTypeViewModel(ProcedureStepType.SendEmail),
							new StepTypeViewModel(ProcedureStepType.ShowMessage),
							new StepTypeViewModel(ProcedureStepType.ControlVisualGet),
							new StepTypeViewModel(ProcedureStepType.ControlVisualSet),
							new StepTypeViewModel(ProcedureStepType.ControlPlanGet),
							new StepTypeViewModel(ProcedureStepType.ControlPlanSet),
							new StepTypeViewModel(ProcedureStepType.ShowDialog),
							new StepTypeViewModel(ProcedureStepType.CloseDialog),
							new StepTypeViewModel(ProcedureStepType.ShowProperty)
						}),
					new StepTypeViewModel("Служебные функции", "/Controls;component/StepIcons/Exit.png",
						new List<StepTypeViewModel>
						{
							new StepTypeViewModel(ProcedureStepType.Exit),
							new StepTypeViewModel(ProcedureStepType.RunProgram),
							new StepTypeViewModel(ProcedureStepType.Pause),
							new StepTypeViewModel(ProcedureStepType.ProcedureSelection),
							new StepTypeViewModel(ProcedureStepType.CheckPermission),
							new StepTypeViewModel(ProcedureStepType.GetJournalItem)
						}),
					new StepTypeViewModel("Функции цикла", "/Controls;component/StepIcons/For.png",
						new List<StepTypeViewModel>
						{
							new StepTypeViewModel(ProcedureStepType.For),
							new StepTypeViewModel(ProcedureStepType.While),
							new StepTypeViewModel(ProcedureStepType.Break),
							new StepTypeViewModel(ProcedureStepType.Continue)
						}),
					new StepTypeViewModel("Управление аппаратурой", "/Controls;component/StepIcons/Control.png",
						new List<StepTypeViewModel>
						{
							new StepTypeViewModel("Управление ГК", "/Controls;component/StepIcons/Control.png",
								new List<StepTypeViewModel>
								{
									new StepTypeViewModel(ProcedureStepType.ControlGKDevice),
									new StepTypeViewModel(ProcedureStepType.ControlGKFireZone),
									new StepTypeViewModel(ProcedureStepType.ControlGKGuardZone),
									new StepTypeViewModel(ProcedureStepType.ControlDirection),
									new StepTypeViewModel(ProcedureStepType.ControlDelay),
									new StepTypeViewModel(ProcedureStepType.ControlPumpStation),
									new StepTypeViewModel(ProcedureStepType.ControlMPT),
									new StepTypeViewModel(ProcedureStepType.ControlGKDoor)
								}),
							new StepTypeViewModel("Управление Видео", "/Controls;component/StepIcons/Control.png",
								new List<StepTypeViewModel>
								{
									new StepTypeViewModel(ProcedureStepType.StartRecord),
									new StepTypeViewModel(ProcedureStepType.StopRecord),
									new StepTypeViewModel(ProcedureStepType.Ptz),
									new StepTypeViewModel(ProcedureStepType.RviAlarm),
									new StepTypeViewModel(ProcedureStepType.RviOpenWindow)
								}),
						}),
					new StepTypeViewModel("OPC DA Сервер", "/Controls;component/StepIcons/Control.png",
						new List<StepTypeViewModel>
						{
							new StepTypeViewModel(ProcedureStepType.ControlOpcDaTagGet),
							new StepTypeViewModel(ProcedureStepType.ControlOpcDaTagSet)
						}),
					new StepTypeViewModel("Обмен данными", "/Controls;component/StepIcons/Export.png",
						new List<StepTypeViewModel>
						{
							new StepTypeViewModel(ProcedureStepType.HttpRequest),
							new StepTypeViewModel(ProcedureStepType.ExportJournal),
							new StepTypeViewModel(ProcedureStepType.ExportConfiguration),
							new StepTypeViewModel(ProcedureStepType.ExportOrganisation),
							new StepTypeViewModel(ProcedureStepType.ExportOrganisationList),
							new StepTypeViewModel(ProcedureStepType.ImportOrganisation),
							new StepTypeViewModel(ProcedureStepType.ImportOrganisationList),
						}),

				});
		}

		protected override bool CanSave()
		{
			return ((SelectedStepType != null) && (!SelectedStepType.IsFolder));
		}
	}
}