using System.Collections.ObjectModel;
using System.Linq;
using StrazhAPI.Automation;
using FiresecClient;
using Localization.Automation.Common;
using Localization.Automation.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ProcedureSelectionStepViewModel : BaseStepViewModel
	{
		ProcedureSelectionArguments ProcedureSelectionArguments { get; set; }

		public ProcedureSelectionStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ProcedureSelectionArguments = stepViewModel.Step.ProcedureSelectionArguments;
		}

		public override void UpdateContent()
		{			
			ScheduleProcedures = new ObservableCollection<ScheduleProcedureViewModel>();
			foreach (var procedure in FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures.FindAll(x => x.Uid != Procedure.Uid))
			{
				var scheduleProcedure = new ScheduleProcedure { ProcedureUid = procedure.Uid };
				if (procedure.Uid == ProcedureSelectionArguments.ScheduleProcedure.ProcedureUid)
					scheduleProcedure = ProcedureSelectionArguments.ScheduleProcedure;
				ScheduleProcedures.Add(new ScheduleProcedureViewModel(scheduleProcedure, Procedure));
			}
			SelectedScheduleProcedure = ScheduleProcedures.FirstOrDefault(x => x.ScheduleProcedure.ProcedureUid == ProcedureSelectionArguments.ScheduleProcedure.ProcedureUid);
			OnPropertyChanged(() => ScheduleProcedures);			
		}

		public ObservableCollection<ScheduleProcedureViewModel> ScheduleProcedures { get; private set; }
		ScheduleProcedureViewModel _selectedScheduleProcedure;
		public ScheduleProcedureViewModel SelectedScheduleProcedure
		{
			get { return _selectedScheduleProcedure; }
			set
			{
				_selectedScheduleProcedure = value;
				if (value != null)
					ProcedureSelectionArguments.ScheduleProcedure = value.ScheduleProcedure;
				if (UpdateDescriptionHandler != null)
					UpdateDescriptionHandler();
				OnPropertyChanged(() => SelectedScheduleProcedure);
			}
		}

		public override string Description
		{
			get 
			{
				return string.Format(StepCommonViewModel.ProcedureSelection ,(SelectedScheduleProcedure != null ? SelectedScheduleProcedure.Name : CommonResources.Empty)); 
			}
		}
	}
}
