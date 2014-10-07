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
			SKDZoneArgument = new ArgumentViewModel(ControlSKDZoneArguments.SKDZoneArgument, stepViewModel.Update);
			SKDZoneArgument.ObjectType = ObjectType.SKDZone;
			SKDZoneArgument.ExplicitType = ExplicitType.Object;
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
			SKDZoneArgument.Update(ProcedureHelper.GetAllVariables(Procedure, ExplicitType.Object, ObjectType.SKDZone, false));
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
