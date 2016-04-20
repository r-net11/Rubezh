using FiresecAPI.Automation;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ProcedureStepViewModel : BaseViewModel
	{
		public ProcedureStep ProcedureStep { get; private set; }
		public SoundStepViewModel SoundStepViewModel { get; private set; }

		public ProcedureStepViewModel(ProcedureStep procedureStep)
		{
			ProcedureStep = procedureStep;

			if (ProcedureStep.ProcedureStepType == ProcedureStepType.PlaySound)
			{
				SoundStepViewModel = new SoundStepViewModel(procedureStep);
			}
		}

		public ProcedureStepType ProcedureStepType
		{
			get { return ProcedureStep.ProcedureStepType; }
		}
	}
}