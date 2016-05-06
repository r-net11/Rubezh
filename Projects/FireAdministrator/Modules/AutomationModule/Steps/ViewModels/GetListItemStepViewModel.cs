using System.Collections.ObjectModel;
using StrazhAPI;
using StrazhAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class GetListItemStepViewModel : BaseStepViewModel
	{
		GetListItemArguments GetListItemArguments { get; set; }
		public ArgumentViewModel ListArgument { get; set; }
		public ArgumentViewModel ItemArgument { get; set; }
		public ArgumentViewModel IndexArgument { get; set; }

		public GetListItemStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			GetListItemArguments = stepViewModel.Step.GetListItemArguments;
			ListArgument = new ArgumentViewModel(GetListItemArguments.ListArgument, stepViewModel.Update, UpdateContent, false);
			ListArgument.UpdateVariableHandler += UpdateItemVariable;
			ItemArgument = new ArgumentViewModel(GetListItemArguments.ItemArgument, stepViewModel.Update, UpdateContent, false);
			IndexArgument = new ArgumentViewModel(GetListItemArguments.IndexArgument, stepViewModel.Update, UpdateContent);
			PositionTypes = ProcedureHelper.GetEnumObs<PositionType>();
		}

		public override void UpdateContent()
		{
			ListArgument.Update(Procedure);
			IndexArgument.Update(Procedure, ExplicitType.Integer);
		}

		void UpdateItemVariable()
		{
			ItemArgument.Update(Procedure, ListArgument.ExplicitType, ListArgument.EnumType, ListArgument.ObjectType);
		}

		public ObservableCollection<PositionType> PositionTypes { get; private set; }
		public PositionType SelectedPositionType
		{
			get { return GetListItemArguments.PositionType; }
			set
			{
				GetListItemArguments.PositionType = value;
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