using RubezhAPI.Automation;
using RubezhClient;
using System.Collections.ObjectModel;
using System.Linq;

namespace AutomationModule.ViewModels
{
	public class ProcedureSelectionStepViewModel : BaseStepViewModel
	{
		ProcedureSelectionStep ProcedureSelectionStep { get; set; }

		public ProcedureSelectionStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ProcedureSelectionStep = (ProcedureSelectionStep)stepViewModel.Step;
		}

		public override void UpdateContent()
		{
			ScheduleProcedures = new ObservableCollection<ScheduleProcedureViewModel>();
			foreach (var procedure in ClientManager.SystemConfiguration.AutomationConfiguration.Procedures.FindAll(x => x.Uid != Procedure.Uid))
			{
				var scheduleProcedure = new ScheduleProcedure { ProcedureUid = procedure.Uid };
				if (procedure.Uid == ProcedureSelectionStep.ScheduleProcedure.ProcedureUid)
					scheduleProcedure = ProcedureSelectionStep.ScheduleProcedure;
				ScheduleProcedures.Add(new ScheduleProcedureViewModel(scheduleProcedure, Procedure));
			}
			SelectedScheduleProcedure = ScheduleProcedures.FirstOrDefault(x => x.ScheduleProcedure.ProcedureUid == ProcedureSelectionStep.ScheduleProcedure.ProcedureUid);
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
					ProcedureSelectionStep.ScheduleProcedure = value.ScheduleProcedure;
				if (UpdateDescriptionHandler != null)
					UpdateDescriptionHandler();
				OnPropertyChanged(() => SelectedScheduleProcedure);
			}
		}

		public override string Description
		{
			get
			{
				return "Процедура: " + (SelectedScheduleProcedure != null ? SelectedScheduleProcedure.Name : "<пусто>");
			}
		}
	}
}
