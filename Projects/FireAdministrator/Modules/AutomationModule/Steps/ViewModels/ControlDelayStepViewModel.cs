using Infrastructure.Automation;
using RubezhAPI;
using RubezhAPI.Automation;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class ControlDelayStepViewModel : BaseStepViewModel
	{
		ControlDelayStep ControlDelayStep { get; set; }
		public ArgumentViewModel DelayArgument { get; private set; }

		public ControlDelayStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ControlDelayStep = (ControlDelayStep)stepViewModel.Step;
			Commands = AutomationHelper.GetEnumObs<DelayCommandType>();
			DelayArgument = new ArgumentViewModel(ControlDelayStep.DelayArgument, stepViewModel.Update, null);
			SelectedCommand = ControlDelayStep.DelayCommandType;
		}

		public ObservableCollection<DelayCommandType> Commands { get; private set; }

		DelayCommandType _selectedCommand;
		public DelayCommandType SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				ControlDelayStep.DelayCommandType = value;
				OnPropertyChanged(() => SelectedCommand);
			}
		}

		public override void UpdateContent()
		{
			DelayArgument.Update(Procedure, ExplicitType.Object, objectType: ObjectType.Delay, isList: false);
		}

		public override string Description
		{
			get
			{
				return "Задержка: " + DelayArgument.Description + " Команда: " + SelectedCommand.ToDescription();
			}
		}
	}
}
