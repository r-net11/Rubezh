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
							new StepTypeViewModel(ProcedureStepType.Random),
							new StepTypeViewModel(ProcedureStepType.GetDateTimeNow),
							new StepTypeViewModel(ProcedureStepType.GenerateGuid)
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
							new StepTypeViewModel(ProcedureStepType.ShowDialog)
						}),
					new StepTypeViewModel("Служебные функции", "/Controls;component/StepIcons/Exit.png",
						new List<StepTypeViewModel>
						{
							new StepTypeViewModel(ProcedureStepType.Exit),
							new StepTypeViewModel(ProcedureStepType.RunProgram),
							new StepTypeViewModel(ProcedureStepType.Pause),
							new StepTypeViewModel(ProcedureStepType.ProcedureSelection),
							new StepTypeViewModel(ProcedureStepType.CheckPermission),
							new StepTypeViewModel(ProcedureStepType.GetJournalItem),
							new StepTypeViewModel(ProcedureStepType.GetSkdDeviceProperty),
							new StepTypeViewModel(ProcedureStepType.GetDoorProperty),
							new StepTypeViewModel(ProcedureStepType.GetSkdZoneProperty),
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
							new StepTypeViewModel("Управление Страж", "/Controls;component/StepIcons/Control.png",
								new List<StepTypeViewModel>
								{
									new StepTypeViewModel(ProcedureStepType.ControlSKDDevice),
									new StepTypeViewModel(ProcedureStepType.ControlSKDZone),
									new StepTypeViewModel(ProcedureStepType.ControlDoor),
								}),
							new StepTypeViewModel("Управление Видео", "/Controls;component/StepIcons/Control.png",
								new List<StepTypeViewModel>
								{
									new StepTypeViewModel(ProcedureStepType.StartRecord),
									new StepTypeViewModel(ProcedureStepType.Ptz),
									new StepTypeViewModel(ProcedureStepType.RviAlarm),
								}),
						}),
					new StepTypeViewModel("Обмен данными", "/Controls;component/StepIcons/Export.png",
						new List<StepTypeViewModel>
						{
							new StepTypeViewModel(ProcedureStepType.ExportReport),
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