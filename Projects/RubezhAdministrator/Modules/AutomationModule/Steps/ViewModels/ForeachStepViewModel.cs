using RubezhAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class ForeachStepViewModel : BaseStepViewModel
	{
		public ForeachStep ForeachStep { get; private set; }
		public ArgumentViewModel ListArgument { get; private set; }
		public ArgumentViewModel ItemArgument { get; private set; }

		public ForeachStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ForeachStep = (ForeachStep)stepViewModel.Step;
			ListArgument = new ArgumentViewModel(ForeachStep.ListArgument, stepViewModel.Update, UpdateContent, false);
			ListArgument.UpdateVariableHandler += UpdateItemVariable;
			ItemArgument = new ArgumentViewModel(ForeachStep.ItemArgument, stepViewModel.Update, UpdateContent, false);
		}

		public override void UpdateContent()
		{
			ListArgument.Update(Procedure, isList: true);
		}

		void UpdateItemVariable()
		{
			ItemArgument.Update(Procedure, ListArgument.ExplicitType, ListArgument.EnumType, ListArgument.ObjectType, false);
		}

		public override string Description
		{
			get { return "Список: " + ListArgument.Description + " Элемент: " + ItemArgument.Description; }
		}

	}
}
