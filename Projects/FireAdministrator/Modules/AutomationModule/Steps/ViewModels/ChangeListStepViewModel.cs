using StrazhAPI;
using StrazhAPI.Automation;
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
			ListArgument = new ArgumentViewModel(ChangeListArguments.ListArgument, stepViewModel.Update, UpdateContent, false)
			{
				UpdateVariableHandler = UpdateItemArgument
			};
			ItemArgument = new ArgumentViewModel(ChangeListArguments.ItemArgument, stepViewModel.Update, UpdateContent);
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
			ListArgument.Update(Procedure);
		}

		void UpdateItemArgument()
		{
			ItemArgument.Update(Procedure, ListArgument.ExplicitType, ListArgument.EnumType, ListArgument.ObjectType);
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
