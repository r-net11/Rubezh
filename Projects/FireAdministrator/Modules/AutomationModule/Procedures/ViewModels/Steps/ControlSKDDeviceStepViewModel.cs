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
using ValueType = FiresecAPI.Automation.ValueType;

namespace AutomationModule.ViewModels
{
	public class ControlSKDDeviceStepViewModel: BaseStepViewModel
	{
		ControlSKDDeviceArguments ControlSKDDeviceArguments { get; set; }
		Procedure Procedure { get; set; }
		public ArithmeticParameterViewModel Variable1 { get; private set; }

		public ControlSKDDeviceStepViewModel(ControlSKDDeviceArguments controlSKDDeviceArguments, Procedure procedure, Action updateDescriptionHandler)
			: base(updateDescriptionHandler)
		{
			ControlSKDDeviceArguments = controlSKDDeviceArguments;
			Procedure = procedure;
			Commands = ProcedureHelper.GetEnumObs<SKDDeviceCommandType>();
			Variable1 = new ArithmeticParameterViewModel(ControlSKDDeviceArguments.Variable1);
			Variable1.ObjectType = ObjectType.SKDDevice;
			Variable1.ValueType = ValueType.Object;
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
			Variable1.Update(ProcedureHelper.GetAllVariables(Procedure, ValueType.Object, ObjectType.SKDDevice, false));
		}

		public override string Description
		{
			get { return ""; }
		}
	}
}

