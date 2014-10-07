using System.Collections.ObjectModel;
using FiresecAPI;
using FiresecAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class GetListItemStepViewModel : BaseStepViewModel
	{
		GetListItemArgument GetListItemArgument { get; set; }
		public ArgumentViewModel ListArgument { get; set; }
		public ArgumentViewModel ItemArgument { get; set; }
		public ArgumentViewModel IndexArgument { get; set; }

		public GetListItemStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			GetListItemArgument = stepViewModel.Step.GetListItemArgument;
			ListArgument = new ArgumentViewModel(GetListItemArgument.ListArgument, stepViewModel.Update, false);
			ListArgument.UpdateVariableHandler += UpdateItemVariable;
			ItemArgument = new ArgumentViewModel(GetListItemArgument.ItemArgument, stepViewModel.Update, false);
			IndexArgument = new ArgumentViewModel(GetListItemArgument.IndexArgument, stepViewModel.Update);
			IndexArgument.ExplicitType = ExplicitType.Integer;
			PositionTypes = ProcedureHelper.GetEnumObs<PositionType>();
		}

		public override void UpdateContent()
		{
			ListArgument.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.IsList));
			IndexArgument.Update(ProcedureHelper.GetAllVariables(Procedure, ExplicitType.Integer).FindAll(x => !x.IsList));
		}

		void UpdateItemVariable()
		{
			ItemArgument.Update(ProcedureHelper.GetAllVariables(Procedure, ListArgument.ExplicitType, ListArgument.ObjectType, ListArgument.EnumType).FindAll(x => !x.IsList));
		}

		public ObservableCollection<PositionType> PositionTypes { get; private set; } 
		public PositionType SelectedPositionType
		{
			get { return GetListItemArgument.PositionType; }
			set
			{
				GetListItemArgument.PositionType = value;
				OnPropertyChanged(() => SelectedPositionType);
			}
		}

		public override string Description
		{
			get
			{
				return "Список: " + ListArgument.Description + " Элемент: " + ItemArgument.Description + " Позиция: " + SelectedPositionType.ToDescription() + (SelectedPositionType == PositionType.ByIndex ? "  [" + IndexArgument.Description + "]" : "");
			}
		}
	}
}