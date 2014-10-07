using FiresecAPI.Automation;

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
			ListArgument = new ArgumentViewModel(ForeachArguments.ListArgument, stepViewModel.Update, false);
			ListArgument.UpdateVariableHandler += UpdateItemVariable;
			ItemArgument = new ArgumentViewModel(ForeachArguments.ItemArgument, stepViewModel.Update, false);
		}

		public override void UpdateContent()
		{
			ListArgument.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.IsList));
		}

		void UpdateItemVariable()
		{
			ItemArgument.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => !x.IsList && x.ExplicitType == ListArgument.SelectedVariable.Variable.ExplicitType
				&& x.ObjectType == ListArgument.SelectedVariable.Variable.ObjectType && x.EnumType == ListArgument.SelectedVariable.Variable.EnumType));
		}

		public override string Description
		{
			get { return "Список: " + ListArgument.Description + " Элемент: " + ItemArgument.Description; }
		}

	}
}
