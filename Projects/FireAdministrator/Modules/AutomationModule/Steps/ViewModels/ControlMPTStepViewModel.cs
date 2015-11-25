using System.Collections.ObjectModel;
using RubezhAPI.Automation;
using RubezhAPI;
using Infrastructure.Automation;

namespace AutomationModule.ViewModels
{
	public class ControlMPTStepViewModel : BaseStepViewModel
	{
		ControlMPTArguments ControlMPTArguments { get; set; }
		public ArgumentViewModel MPTArgument { get; private set; }

		public ControlMPTStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ControlMPTArguments = stepViewModel.Step.ControlMPTArguments;
			Commands = AutomationHelper.GetEnumObs<MPTCommandType>();
			MPTArgument = new ArgumentViewModel(ControlMPTArguments.MPTArgument, stepViewModel.Update, null);
			SelectedCommand = ControlMPTArguments.MPTCommandType;
		}

		public ObservableCollection<MPTCommandType> Commands { get; private set; }
		MPTCommandType _selectedCommand;
		public MPTCommandType SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				ControlMPTArguments.MPTCommandType = value;
				OnPropertyChanged(() => SelectedCommand);
			}
		}

		public override void UpdateContent()
		{
			MPTArgument.Update(Procedure, ExplicitType.Object, objectType: ObjectType.MPT, isList: false);
		}

		public override string Description
		{
			get
			{
				return "МПТ: " + MPTArgument.Description + " Команда: " + SelectedCommand.ToDescription();
			}
		}
	}
}
