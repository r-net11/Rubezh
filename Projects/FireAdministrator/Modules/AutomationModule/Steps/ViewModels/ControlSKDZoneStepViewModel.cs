using System.Collections.ObjectModel;
using FiresecAPI.Automation;
using FiresecAPI;

namespace AutomationModule.ViewModels
{
	public class ControlSKDZoneStepViewModel: BaseStepViewModel
	{
		ControlSKDZoneArguments ControlSKDZoneArguments { get; set; }
		public ArgumentViewModel SKDZoneArgument { get; private set; }

		public ControlSKDZoneStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ControlSKDZoneArguments = stepViewModel.Step.ControlSKDZoneArguments;
			Commands = ProcedureHelper.GetEnumObs<SKDZoneCommandType>();
			SKDZoneArgument = new ArgumentViewModel(ControlSKDZoneArguments.SKDZoneArgument, stepViewModel.Update, null);
			SelectedCommand = ControlSKDZoneArguments.SKDZoneCommandType;
		}

		public ObservableCollection<SKDZoneCommandType> Commands { get; private set; }
		SKDZoneCommandType _selectedCommand;
		public SKDZoneCommandType SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				ControlSKDZoneArguments.SKDZoneCommandType = value;
				OnPropertyChanged(()=>SelectedCommand);
			}
		}

		public override void UpdateContent()
		{
			SKDZoneArgument.Update(Procedure, ExplicitType.Object, objectType:ObjectType.SKDZone);
		}

		public override string Description
		{
			get
			{
				return "Зона: " + SKDZoneArgument.Description + " Команда: " + SelectedCommand.ToDescription();
			}
		}
	}
}
