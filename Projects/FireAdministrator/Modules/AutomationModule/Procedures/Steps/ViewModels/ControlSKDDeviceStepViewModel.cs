using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Automation;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ControlSKDDeviceStepViewModel: BaseStepViewModel
	{
		ControlSKDDeviceArguments ControlSKDDeviceArguments { get; set; }
		public ArgumentViewModel SKDDeviceParameter { get; private set; }

		public ControlSKDDeviceStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ControlSKDDeviceArguments = stepViewModel.Step.ControlSKDDeviceArguments;
			Commands = ProcedureHelper.GetEnumObs<SKDDeviceCommandType>();
			SKDDeviceParameter = new ArgumentViewModel(ControlSKDDeviceArguments.SKDDeviceParameter, stepViewModel.Update);
			SKDDeviceParameter.ObjectType = ObjectType.SKDDevice;
			SKDDeviceParameter.ExplicitType = ExplicitType.Object;
			SelectedCommand = ControlSKDDeviceArguments.Command;
			UpdateContent();
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
			SKDDeviceParameter.Update(ProcedureHelper.GetAllVariables(Procedure, ExplicitType.Object, ObjectType.SKDDevice, false));
		}

		public override string Description
		{
			get
			{
				return "Устройство: " + SKDDeviceParameter.Description + " Команда: " + SelectedCommand.ToDescription();
			}
		}
	}
}

