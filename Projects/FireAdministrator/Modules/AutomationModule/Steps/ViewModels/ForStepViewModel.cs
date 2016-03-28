using RubezhAPI.Automation;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	class ForStepViewModel : BaseStepViewModel
	{
		public ForStep ForStep { get; private set; }
		public ArgumentViewModel IndexerArgument { get; private set; }
		public ArgumentViewModel InitialValueArgument { get; private set; }
		public ArgumentViewModel ValueArgument { get; private set; }
		public ArgumentViewModel IteratorArgument { get; private set; }

		public ForStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ForStep = (ForStep)stepViewModel.Step;
			ConditionTypes = new ObservableCollection<ConditionType> { ConditionType.IsLess, ConditionType.IsNotMore, ConditionType.IsMore, ConditionType.IsNotLess };
			SelectedConditionType = ForStep.ConditionType;
			IndexerArgument = new ArgumentViewModel(ForStep.IndexerArgument, stepViewModel.Update, UpdateContent, false, true, false);
			InitialValueArgument = new ArgumentViewModel(ForStep.InitialValueArgument, stepViewModel.Update, UpdateContent);
			ValueArgument = new ArgumentViewModel(ForStep.ValueArgument, stepViewModel.Update, UpdateContent);
			IteratorArgument = new ArgumentViewModel(ForStep.IteratorArgument, stepViewModel.Update, UpdateContent);
		}

		public override void UpdateContent()
		{
			IndexerArgument.Update(Procedure, ExplicitType.Integer, isList: false);
			InitialValueArgument.Update(Procedure, ExplicitType.Integer, isList: false);
			ValueArgument.Update(Procedure, ExplicitType.Integer, isList: false);
			IteratorArgument.Update(Procedure, ExplicitType.Integer, isList: false);
		}

		public ObservableCollection<ConditionType> ConditionTypes { get; private set; }
		public ConditionType SelectedConditionType
		{
			get { return ForStep.ConditionType; }
			set
			{
				ForStep.ConditionType = value;
				OnPropertyChanged(() => SelectedConditionType);
			}
		}

		public override string Description
		{
			get
			{
				var op = "";
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
