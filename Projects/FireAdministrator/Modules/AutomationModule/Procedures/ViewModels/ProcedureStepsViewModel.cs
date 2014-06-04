using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.XModels.Automation;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ProcedureStepsViewModel : SaveCancelDialogViewModel
	{
		public ObservableCollection<ProcedureStep>  ProcedureSteps { get; private set; }

		public ProcedureStepsViewModel()
		{
			Title = "Список действий";
			ProcedureSteps = new ObservableCollection<ProcedureStep>
			{
				new ProcedureStep {ProcedureStepType = ProcedureStepType.PlaySound},
				new ProcedureStep {ProcedureStepType = ProcedureStepType.DoAction}
			};
			SelectedProcedureStep = ProcedureSteps.FirstOrDefault();
		}

		ProcedureStep _selectedProcedureStep;
		public ProcedureStep SelectedProcedureStep
		{
			get { return _selectedProcedureStep; }
			set
			{
				_selectedProcedureStep = value;
				OnPropertyChanged(() => SelectedProcedureStep);
			}
		}

		protected override bool Save()
		{
			return base.Save();
		}
	}
}
