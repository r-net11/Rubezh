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
			ListArgument = new ArgumentViewModel(GetListItemArgument.ListArgument, stepViewModel.Update, UpdateContent, false);
			ListArgument.UpdateVariableHandler += UpdateItemVariable;
			ItemArgument = new ArgumentViewModel(GetListItemArgument.ItemArgument, stepViewModel.Update, UpdateContent, false);
			IndexArgument = new ArgumentViewModel(GetListItemArgument.IndexArgument, stepViewModel.Update, UpdateContent);
			PositionTypes = ProcedureHelper.GetEnumObs<PositionType>();
		}

		public override void UpdateContent()
		{
			ListArgument.Update(Procedure, isList:true);
			IndexArgument.Update(Procedure, ExplicitType.Integer, isList:false);
		}

		void UpdateItemVariable()
		{
			ItemArgument.Update(Procedure, ListArgument.ExplicitType, ListArgument.EnumType, ListArgument.ObjectType, false);
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