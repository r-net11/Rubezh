using System.Collections.ObjectModel;
using RubezhAPI.Automation;
using RubezhAPI;
using Infrastructure.Automation;

namespace AutomationModule.ViewModels
{
	public class ControlPumpStationStepViewModel : BaseStepViewModel
	{
		ControlPumpStationArguments ControlPumpStationArguments { get; set; }
		public ArgumentViewModel PumpStationArgument { get; private set; }

		public ControlPumpStationStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ControlPumpStationArguments = stepViewModel.Step.ControlPumpStationArguments;
			Commands = AutomationHelper.GetEnumObs<PumpStationCommandType>();
			PumpStationArgument = new ArgumentViewModel(ControlPumpStationArguments.PumpStationArgument, stepViewModel.Update, null);
			SelectedCommand = ControlPumpStationArguments.PumpStationCommandType;
		}

		public ObservableCollection<PumpStationCommandType> Commands { get; private set; }
		PumpStationCommandType _selectedCommand;
		public PumpStationCommandType SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				ControlPumpStationArguments.PumpStationCommandType = value;
				OnPropertyChanged(() => SelectedCommand);
			}
		}

		public override void UpdateContent()
		{
			PumpStationArgument.Update(Procedure, ExplicitType.Object, objectType: ObjectType.PumpStation, isList: false);
		}

		public override string Description
		{
			get
			{
				return "Насосная станция: " + PumpStationArgument.Description + " Команда: " + SelectedCommand.ToDescription();
			}
		}
	}
}
