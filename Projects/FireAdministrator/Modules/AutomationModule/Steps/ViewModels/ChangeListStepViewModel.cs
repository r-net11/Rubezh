using FiresecAPI;
using FiresecAPI.Automation;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class ChangeListStepViewModel : BaseStepViewModel
	{
		ChangeListArguments ChangeListArguments { get; set; }
		public ArgumentViewModel ListArgument { get; private set; }
		public ArgumentViewModel ItemArgument { get; private set; }

		public ChangeListStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ChangeListArguments = stepViewModel.Step.ChangeListArguments;
			ListArgument = new ArgumentViewModel(ChangeListArguments.ListArgument, stepViewModel.Update, false);
			ListArgument.UpdateVariableHandler = UpdateItemArgument;
			ItemArgument = new ArgumentViewModel(ChangeListArguments.ItemArgument, stepViewModel.Update);
			ChangeTypes = ProcedureHelper.GetEnumObs<ChangeType>();
		}

		public ObservableCollection<ChangeType> ChangeTypes { get; private set; }
		public ChangeType SelectedChangeType
		{
			get { return ChangeListArguments.ChangeType; }
			set
			{
				ChangeListArguments.ChangeType = value;
				OnPropertyChanged(() => SelectedChangeType);
			}
		}

		public override void UpdateContent()
		{
			ListArgument.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.IsList));
		}

		void UpdateItemArgument()
		{
			var allVariables = ProcedureHelper.GetAllVariables(Procedure).FindAll(x => !x.IsList && x.ExplicitType == ListArgument.ExplicitType);
			if (ListArgument.ExplicitType == ExplicitType.Object)
				allVariables = allVariables.FindAll(x => x.ObjectType == ListArgument.ObjectType);
			if (ListArgument.ExplicitType == ExplicitType.Enum)
				allVariables = allVariables.FindAll(x => x.EnumType == ListArgument.EnumType);
			ItemArgument.Update(allVariables);
			ItemArgument.ExplicitType = ListArgument.ExplicitType;
			ItemArgument.ObjectType = ListArgument.ObjectType;
			ItemArgument.EnumType = ListArgument.EnumType;
		}

		public override string Description
		{
			get
			{
				return "Список: " + ListArgument.Description + " Элемент: " + ItemArgument.Description + " Операция: " + ChangeListArguments.ChangeType.ToDescription();
			}
		}
	}
}
