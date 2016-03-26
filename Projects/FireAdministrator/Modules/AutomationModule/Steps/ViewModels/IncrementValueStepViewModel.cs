using Infrastructure.Automation;
using RubezhAPI;
using RubezhAPI.Automation;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class IncrementValueStepViewModel : BaseStepViewModel
	{
		IncrementValueStep IncrementValueStep { get; set; }
		public ArgumentViewModel ResultArgument { get; private set; }

		public IncrementValueStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			IncrementValueStep = (IncrementValueStep)stepViewModel.Step;
			IncrementTypes = AutomationHelper.GetEnumObs<IncrementType>();
			ResultArgument = new ArgumentViewModel(IncrementValueStep.ResultArgument, stepViewModel.Update, null, false);
		}

		public ObservableCollection<IncrementType> IncrementTypes { get; private set; }
		public IncrementType SelectedIncrementType
		{
			get { return IncrementValueStep.IncrementType; }
			set
			{
				IncrementValueStep.IncrementType = value;
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
