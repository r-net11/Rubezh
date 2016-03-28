using Infrastructure.Automation;
using RubezhAPI;
using RubezhAPI.Automation;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class ControlDirectionStepViewModel : BaseStepViewModel
	{
		ControlDirectionStep ControlDirectionStep { get; set; }
		public ArgumentViewModel DirectionArgument { get; private set; }

		public ControlDirectionStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ControlDirectionStep = (ControlDirectionStep)stepViewModel.Step;
			Commands = AutomationHelper.GetEnumObs<DirectionCommandType>();
			DirectionArgument = new ArgumentViewModel(ControlDirectionStep.DirectionArgument, stepViewModel.Update, null);
			SelectedCommand = ControlDirectionStep.DirectionCommandType;
		}

		public ObservableCollection<DirectionCommandType> Commands { get; private set; }
		DirectionCommandType _selectedCommand;
		public DirectionCommandType SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				ControlDirectionStep.DirectionCommandType = value;
				OnPropertyChanged(() => SelectedCommand);
			}
		}

		public override void UpdateContent()
		{
			DirectionArgument.Update(Procedure, ExplicitType.Object, objectType: ObjectType.Direction, isList: false);
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
