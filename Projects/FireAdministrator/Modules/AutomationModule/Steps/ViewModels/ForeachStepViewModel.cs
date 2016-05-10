using StrazhAPI.Automation;
using Localization.Automation.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ForeachStepViewModel : BaseStepViewModel
	{
		public ForeachArguments ForeachArguments { get; private set; }
		public ArgumentViewModel ListArgument { get; private set; }
		public ArgumentViewModel ItemArgument { get; private set; }

		public ForeachStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ForeachArguments = stepViewModel.Step.ForeachArguments;
			ListArgument = new ArgumentViewModel(ForeachArguments.ListArgument, stepViewModel.Update, UpdateContent, false);
			ListArgument.UpdateVariableHandler += UpdateItemVariable;
			ItemArgument = new ArgumentViewModel(ForeachArguments.ItemArgument, stepViewModel.Update, UpdateContent, false);
		}

		public override void UpdateContent()
		{
			ListArgument.Update(Procedure);
		}

		void UpdateItemVariable()
		{
			ItemArgument.Update(Procedure, ListArgument.ExplicitType, ListArgument.EnumType, ListArgument.ObjectType);
		}

		public override string Description
		{
			get { return string.Format(StepCommonViewModel.Foreach,ListArgument.Description, ItemArgument.Description); }
		}

	}
}
