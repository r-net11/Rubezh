using Infrastructure.Automation;
using RubezhAPI;
using RubezhAPI.Automation;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class GetListItemStepViewModel : BaseStepViewModel
	{
		GetListItemStep GetListItemStep { get; set; }
		public ArgumentViewModel ListArgument { get; set; }
		public ArgumentViewModel ItemArgument { get; set; }
		public ArgumentViewModel IndexArgument { get; set; }

		public GetListItemStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			GetListItemStep = (GetListItemStep)stepViewModel.Step;
			ListArgument = new ArgumentViewModel(GetListItemStep.ListArgument, stepViewModel.Update, UpdateContent, false);
			ListArgument.UpdateVariableHandler += UpdateItemVariable;
			ItemArgument = new ArgumentViewModel(GetListItemStep.ItemArgument, stepViewModel.Update, UpdateContent, false);
			IndexArgument = new ArgumentViewModel(GetListItemStep.IndexArgument, stepViewModel.Update, UpdateContent);
			PositionTypes = AutomationHelper.GetEnumObs<PositionType>();
		}

		public override void UpdateContent()
		{
			ListArgument.Update(Procedure, isList: true);
			IndexArgument.Update(Procedure, ExplicitType.Integer, isList: false);
		}

		void UpdateItemVariable()
		{
			ItemArgument.Update(Procedure, ListArgument.ExplicitType, ListArgument.EnumType, ListArgument.ObjectType, false);
		}

		public ObservableCollection<PositionType> PositionTypes { get; private set; }
		public PositionType SelectedPositionType
		{
			get { return GetListItemStep.PositionType; }
			set
			{
				GetListItemStep.PositionType = value;
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