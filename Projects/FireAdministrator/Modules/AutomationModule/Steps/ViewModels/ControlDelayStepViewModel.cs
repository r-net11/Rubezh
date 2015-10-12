using System.Collections.ObjectModel;
using RubezhAPI.Automation;
using RubezhAPI;
using Infrastructure.Automation;

namespace AutomationModule.ViewModels
{
	public class ControlDelayStepViewModel: BaseStepViewModel
	{
		ControlDelayArguments ControlDelayArguments { get; set; }
		public ArgumentViewModel DelayArgument { get; private set; }

		public ControlDelayStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ControlDelayArguments = stepViewModel.Step.ControlDelayArguments;
			Commands = AutomationHelper.GetEnumObs<DelayCommandType>();
			DelayArgument = new ArgumentViewModel(ControlDelayArguments.DelayArgument, stepViewModel.Update, null);
			SelectedCommand = ControlDelayArguments.DelayCommandType;
		}

		public ObservableCollection<DelayCommandType> Commands { get; private set; }

		DelayCommandType _selectedCommand;
		public DelayCommandType SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				ControlDelayArguments.DelayCommandType = value;
				OnPropertyChanged(()=>SelectedCommand);
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
