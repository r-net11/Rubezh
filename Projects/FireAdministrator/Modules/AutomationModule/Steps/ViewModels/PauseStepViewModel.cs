using FiresecAPI.Automation;
using System.Collections.ObjectModel;
using FiresecAPI;

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
			PauseArgument = new ArgumentViewModel(PauseArguments.PauseArgument, stepViewModel.Update);
			PauseArgument.ExplicitType = ExplicitType.Integer;
		}

		public override void UpdateContent()
		{
			PauseArgument.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.Integer && !x.IsList));
		}

		public override string Description
		{
			get 
			{
				return "Значение: " + PauseArgument.Description + " " + SelectedTimeType.ToDescription(); 
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