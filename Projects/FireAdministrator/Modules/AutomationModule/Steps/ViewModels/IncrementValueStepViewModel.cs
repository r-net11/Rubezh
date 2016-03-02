using Infrastructure.Automation;
using RubezhAPI;
using RubezhAPI.Automation;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class IncrementValueStepViewModel : BaseStepViewModel
	{
		IncrementValueArguments IncrementValueArguments { get; set; }
		public ArgumentViewModel ResultArgument { get; private set; }

		public IncrementValueStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			IncrementValueArguments = stepViewModel.Step.IncrementValueArguments;
			IncrementTypes = AutomationHelper.GetEnumObs<IncrementType>();
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
			ResultArgument.Update(Procedure, new List<ExplicitType> { ExplicitType.Integer, ExplicitType.Float }, isList: false);
		}

		public override string Description
		{
			get
			{
				return "Переменная: " + ResultArgument.Description + " Значение: " + SelectedIncrementType.ToDescription();
			}
		}
	}
}
