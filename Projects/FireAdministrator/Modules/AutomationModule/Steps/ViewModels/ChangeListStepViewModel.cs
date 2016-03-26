using Infrastructure.Automation;
using RubezhAPI;
using RubezhAPI.Automation;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class ChangeListStepViewModel : BaseStepViewModel
	{
		ChangeListStep ChangeListStep { get; set; }
		public ArgumentViewModel ListArgument { get; private set; }
		public ArgumentViewModel ItemArgument { get; private set; }

		public ChangeListStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ChangeListStep = (ChangeListStep)stepViewModel.Step;
			ListArgument = new ArgumentViewModel(ChangeListStep.ListArgument, stepViewModel.Update, UpdateContent, false);
			ListArgument.UpdateVariableHandler = UpdateItemArgument;
			ItemArgument = new ArgumentViewModel(ChangeListStep.ItemArgument, stepViewModel.Update, UpdateContent);
			ChangeTypes = AutomationHelper.GetEnumObs<ChangeType>();
		}

		public ObservableCollection<ChangeType> ChangeTypes { get; private set; }
		public ChangeType SelectedChangeType
		{
			get { return ChangeListStep.ChangeType; }
			set
			{
				ChangeListStep.ChangeType = value;
				OnPropertyChanged(() => SelectedChangeType);
			}
		}

		public override void UpdateContent()
		{
			ListArgument.Update(Procedure, isList: true);
		}

		void UpdateItemArgument()
		{
			ItemArgument.Update(Procedure, ListArgument.ExplicitType, ListArgument.EnumType, ListArgument.ObjectType, false);
		}

		public override string Description
		{
			get
			{
				return "Список: " + ListArgument.Description + " Элемент: " + ItemArgument.Description + " Операция: " + ChangeListStep.ChangeType.ToDescription();
			}
		}
	}
}
