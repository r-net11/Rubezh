using StrazhAPI;
using StrazhAPI.Enums;
using StrazhAPI.Models.Automation.StepArguments;

namespace AutomationModule.ViewModels
{
	public class SendOPCCommandStepViewModel : BaseStepViewModel
	{
		private readonly SendOPCCommandArguments _arguments;

		public OPCCommandType SelectedCommandType
		{
			get { return _arguments.SelectedCommandType; }
			set
			{
				if (_arguments.SelectedCommandType == value) return;
				_arguments.SelectedCommandType = value;
				OnPropertyChanged(() => SelectedCommandType);
			}
		}

		public SendOPCCommandStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			_arguments = stepViewModel.Step.SendOPCCommandArguments;
		}

		public override string Description
		{
			get
			{
				return string.Format("Команда: {0}", SelectedCommandType.ToDescription());
			}
		}
	}
}
