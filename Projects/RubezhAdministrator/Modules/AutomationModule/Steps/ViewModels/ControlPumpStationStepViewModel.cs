using Infrastructure.Automation;
using RubezhAPI;
using RubezhAPI.Automation;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class ControlPumpStationStepViewModel : BaseStepViewModel
	{
		ControlPumpStationStep ControlPumpStationStep { get; set; }
		public ArgumentViewModel PumpStationArgument { get; private set; }

		public ControlPumpStationStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ControlPumpStationStep = (ControlPumpStationStep)stepViewModel.Step;
			Commands = AutomationHelper.GetEnumObs<PumpStationCommandType>();
			PumpStationArgument = new ArgumentViewModel(ControlPumpStationStep.PumpStationArgument, stepViewModel.Update, null);
			SelectedCommand = ControlPumpStationStep.PumpStationCommandType;
		}

		public ObservableCollection<PumpStationCommandType> Commands { get; private set; }
		PumpStationCommandType _selectedCommand;
		public PumpStationCommandType SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				ControlPumpStationStep.PumpStationCommandType = value;
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
