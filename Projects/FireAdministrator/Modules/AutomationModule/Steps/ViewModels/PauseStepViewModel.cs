using StrazhAPI.Automation;
using System.Collections.ObjectModel;
using StrazhAPI;
using Localization.Automation.ViewModels;

namespace AutomationModule.ViewModels
{
	public class PauseStepViewModel : BaseStepViewModel
	{
		public PauseArguments PauseArguments { get; private set; }
		public ArgumentViewModel PauseArgument { get; set; }

		public PauseStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			PauseArguments = stepViewModel.Step.PauseArguments;
			TimeTypes = ProcedureHelper.GetEnumObs<TimeType>();
			PauseArgument = new ArgumentViewModel(PauseArguments.PauseArgument, stepViewModel.Update, null)
			{
				ExplicitValue = {MinIntValue = 0}
			};
		}

		public override void UpdateContent()
		{
			PauseArgument.Update(Procedure, ExplicitType.Integer);
		}

		public override string Description
		{
			get 
			{
				return string.Format(StepCommonViewModel.Pause, PauseArgument.Description,SelectedTimeType.ToDescription()); 
			}
		}

		public ObservableCollection<TimeType> TimeTypes { get; private set; }

		public TimeType SelectedTimeType
		{
			get { return PauseArguments.TimeType; }
			set
			{
				PauseArguments.TimeType = value;
				OnPropertyChanged(() => SelectedTimeType);
			}
		}
	}
}