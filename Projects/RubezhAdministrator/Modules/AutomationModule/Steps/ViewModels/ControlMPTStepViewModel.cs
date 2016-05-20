using Infrastructure.Automation;
using RubezhAPI;
using RubezhAPI.Automation;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class ControlMPTStepViewModel : BaseStepViewModel
	{
		ControlMPTStep ControlMPTStep { get; set; }
		public ArgumentViewModel MPTArgument { get; private set; }

		public ControlMPTStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ControlMPTStep = (ControlMPTStep)stepViewModel.Step;
			Commands = AutomationHelper.GetEnumObs<MPTCommandType>();
			MPTArgument = new ArgumentViewModel(ControlMPTStep.MPTArgument, stepViewModel.Update, null);
			SelectedCommand = ControlMPTStep.MPTCommandType;
		}

		public ObservableCollection<MPTCommandType> Commands { get; private set; }
		MPTCommandType _selectedCommand;
		public MPTCommandType SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				ControlMPTStep.MPTCommandType = value;
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
