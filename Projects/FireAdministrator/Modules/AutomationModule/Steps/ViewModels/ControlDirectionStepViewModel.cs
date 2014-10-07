using System.Collections.ObjectModel;
using FiresecAPI.Automation;
using FiresecAPI;

namespace AutomationModule.ViewModels
{
	public class ControlDirectionStepViewModel : BaseStepViewModel
	{
		ControlDirectionArguments ControlDirectionArguments { get; set; }
		public ArgumentViewModel DirectionArgument { get; private set; }

		public ControlDirectionStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ControlDirectionArguments = stepViewModel.Step.ControlDirectionArguments;
			Commands = ProcedureHelper.GetEnumObs<DirectionCommandType>();
			DirectionArgument = new ArgumentViewModel(ControlDirectionArguments.DirectionArgument, stepViewModel.Update);
			DirectionArgument.ObjectType = ObjectType.Direction;
			DirectionArgument.ExplicitType = ExplicitType.Object;
			SelectedCommand = ControlDirectionArguments.DirectionCommandType;
		}

		public ObservableCollection<DirectionCommandType> Commands { get; private set; }
		DirectionCommandType _selectedCommand;
		public DirectionCommandType SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				ControlDirectionArguments.DirectionCommandType = value;
				OnPropertyChanged(() => SelectedCommand);
			}
		}

		public override void UpdateContent()
		{
			DirectionArgument.Update(ProcedureHelper.GetAllVariables(Procedure, ExplicitType.Object, ObjectType.Direction, false));
		}

		public override string Description
		{
			get
			{
				return "Направление: " + DirectionArgument.Description + " Команда: " + SelectedCommand.ToDescription();
			}
		}
	}
}
