using System.Collections.ObjectModel;
using FiresecAPI.Automation;
using FiresecAPI;

namespace AutomationModule.ViewModels
{
	public class ControlSKDZoneStepViewModel: BaseStepViewModel
	{
		ControlSKDZoneArguments ControlSKDZoneArguments { get; set; }
		public ArgumentViewModel SKDZoneParameter { get; private set; }

		public ControlSKDZoneStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ControlSKDZoneArguments = stepViewModel.Step.ControlSKDZoneArguments;
			Commands = ProcedureHelper.GetEnumObs<SKDZoneCommandType>();
			SKDZoneParameter = new ArgumentViewModel(ControlSKDZoneArguments.SKDZoneParameter, stepViewModel.Update);
			SKDZoneParameter.ObjectType = ObjectType.SKDZone;
			SKDZoneParameter.ExplicitType = ExplicitType.Object;
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
			SKDZoneParameter.Update(ProcedureHelper.GetAllVariables(Procedure, ExplicitType.Object, ObjectType.SKDZone, false));
		}

		public override string Description
		{
			get
			{
				return "Зона: " + SKDZoneParameter.Description + " Команда: " + SelectedCommand.ToDescription();
			}
		}
	}
}
