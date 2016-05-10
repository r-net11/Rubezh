using System.Collections.ObjectModel;
using StrazhAPI.Automation;
using StrazhAPI;
using Localization.Automation.ViewModels;

namespace AutomationModule.ViewModels
{
	public class IncrementValueStepViewModel: BaseStepViewModel
	{
		IncrementValueArguments IncrementValueArguments { get; set; }
		public ArgumentViewModel ResultArgument { get; private set; }

		public IncrementValueStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			IncrementValueArguments = stepViewModel.Step.IncrementValueArguments;
			IncrementTypes = ProcedureHelper.GetEnumObs<IncrementType>();
			ResultArgument = new ArgumentViewModel(IncrementValueArguments.ResultArgument, stepViewModel.Update, null, false);
		}
		
		public ObservableCollection<IncrementType> IncrementTypes { get; private set; }
		public IncrementType SelectedIncrementType
		{
			get { return IncrementValueArguments.IncrementType; }
			set
			{
				IncrementValueArguments.IncrementType = value;
				OnPropertyChanged(() => SelectedIncrementType);
			}
		}

		public override void UpdateContent()
		{			
			ResultArgument.Update(Procedure, ExplicitType.Integer);
		}

		public override string Description 
		{
			get 
			{
				return string.Format(StepCommonViewModel.IncrementValue, ResultArgument.Description,SelectedIncrementType.ToDescription());
			}
		}
	}
}
