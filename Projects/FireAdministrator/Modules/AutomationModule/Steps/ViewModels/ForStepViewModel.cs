using System.Collections.ObjectModel;
using FiresecAPI.Automation;

namespace AutomationModule.ViewModels
{
	class ForStepViewModel : BaseStepViewModel
	{
		public ForArguments ForArguments { get; private set; }
		public ArgumentViewModel IndexerArgument { get; private set; }
		public ArgumentViewModel InitialValueArgument { get; private set; }
		public ArgumentViewModel ValueArgument { get; private set; }
		public ArgumentViewModel IteratorArgument { get; private set; }

		public ForStepViewModel(StepViewModel stepViewModel): base(stepViewModel)
		{
			ForArguments = stepViewModel.Step.ForArguments;
			ConditionTypes = new ObservableCollection<ConditionType>{ConditionType.IsLess, ConditionType.IsNotMore, ConditionType.IsMore, ConditionType.IsNotLess};
			SelectedConditionType = ForArguments.ConditionType;
			IndexerArgument = new ArgumentViewModel(ForArguments.IndexerArgument, stepViewModel.Update, UpdateContent, false, true, false);
			InitialValueArgument = new ArgumentViewModel(ForArguments.InitialValueArgument, stepViewModel.Update, UpdateContent);
			ValueArgument = new ArgumentViewModel(ForArguments.ValueArgument, stepViewModel.Update, UpdateContent);
			IteratorArgument = new ArgumentViewModel(ForArguments.IteratorArgument, stepViewModel.Update, UpdateContent);
		}

		public override void UpdateContent()
		{
			IndexerArgument.Update(Procedure, ExplicitType.Integer);
			InitialValueArgument.Update(Procedure, ExplicitType.Integer);
			ValueArgument.Update(Procedure, ExplicitType.Integer);
			IteratorArgument.Update(Procedure, ExplicitType.Integer);
		}

		public ObservableCollection<ConditionType> ConditionTypes { get; private set; }
		public ConditionType SelectedConditionType
		{
			get { return ForArguments.ConditionType; }
			set
			{
				ForArguments.ConditionType = value;
				OnPropertyChanged(() => SelectedConditionType);
			}
		}

		public override string Description
		{
			get
			{
				var op = string.Empty;
				switch (SelectedConditionType)
				{
					case ConditionType.IsLess:
						op = " < ";
						break;
					case ConditionType.IsNotMore:
						op = " ≤ ";
						break;
					case ConditionType.IsMore:
						op = " > ";
						break;
					case ConditionType.IsNotLess:
						op = " ≥ ";
						break;
				}
				return IndexerArgument.Description + " = " + InitialValueArgument.Description + @"; " +
					IndexerArgument.Description + op + @"; " + ValueArgument.Description + @"; " +
					IndexerArgument.Description + " = " + IndexerArgument.Description + " + " + IteratorArgument.Description;
			}
		}
	}
}
