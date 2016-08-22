using System.Collections.ObjectModel;
using StrazhAPI;
using StrazhAPI.Automation;
using Localization.Automation.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ControlSKDDeviceStepViewModel: BaseStepViewModel
	{
		ControlSKDDeviceArguments ControlSKDDeviceArguments { get; set; }
		public ArgumentViewModel SKDDeviceArgument { get; private set; }

		public ControlSKDDeviceStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ControlSKDDeviceArguments = stepViewModel.Step.ControlSKDDeviceArguments;
			Commands = ProcedureHelper.GetEnumObs<SKDDeviceCommandType>();
			SKDDeviceArgument = new ArgumentViewModel(ControlSKDDeviceArguments.SKDDeviceArgument, stepViewModel.Update, null);
			SelectedCommand = ControlSKDDeviceArguments.Command;
		}

		public ObservableCollection<SKDDeviceCommandType> Commands { get; private set; }
		SKDDeviceCommandType _selectedCommand;
		public SKDDeviceCommandType SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				ControlSKDDeviceArguments.Command = value;
				OnPropertyChanged(()=>SelectedCommand);
			}
		}

		public override void UpdateContent()
		{
			SKDDeviceArgument.Update(Procedure, ExplicitType.Object, objectType:ObjectType.SKDDevice);
		}

		public override string Description
		{
			get
			{
				return string.Format(StepCommonViewModel.ControlSKDDevice, SKDDeviceArgument.Description, SelectedCommand.ToDescription());
			}
		}
	}
}

