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
using FiresecAPI;

namespace AutomationModule.ViewModels
{
	public class ControlSKDDeviceStepViewModel: BaseStepViewModel
	{
		ControlSKDDeviceArguments ControlSKDDeviceArguments { get; set; }
		public ArithmeticParameterViewModel Variable1 { get; private set; }

		public ControlSKDDeviceStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ControlSKDDeviceArguments = stepViewModel.Step.ControlSKDDeviceArguments;
			Commands = ProcedureHelper.GetEnumObs<SKDDeviceCommandType>();
			Variable1 = new ArithmeticParameterViewModel(ControlSKDDeviceArguments.Variable1, stepViewModel.Update);
			Variable1.ObjectType = ObjectType.SKDDevice;
			Variable1.ExplicitType = ExplicitType.Object;
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
			Variable1.Update(ProcedureHelper.GetAllVariables(Procedure, ExplicitType.Object, ObjectType.SKDDevice, false));
		}

		public override string Description
		{
			get
			{
				return "Устройство: " + Variable1.Description + "Команда: " + SelectedCommand.ToDescription();
			}
		}
	}
}

