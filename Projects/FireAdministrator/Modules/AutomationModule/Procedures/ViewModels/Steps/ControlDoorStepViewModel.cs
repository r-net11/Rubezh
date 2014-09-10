using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ValueType = FiresecAPI.Automation.ValueType;

namespace AutomationModule.ViewModels
{
	public class ControlDoorStepViewModel : BaseStepViewModel
	{
		ControlDoorArguments ControlDoorArguments { get; set; }
		Procedure Procedure { get; set; }
		public ArithmeticParameterViewModel Variable1 { get; private set; }

		public ControlDoorStepViewModel(ControlDoorArguments controlDoorArguments, Procedure procedure, Action updateDescriptionHandler)
			: base(updateDescriptionHandler)
		{
			ControlDoorArguments = controlDoorArguments;
			Procedure = procedure;
			Commands = ProcedureHelper.GetEnumObs<DoorCommandType>();
			Variable1 = new ArithmeticParameterViewModel(ControlDoorArguments.Variable1);
			Variable1.ObjectType = ObjectType.ControlDoor;
			Variable1.ValueType = ValueType.Object;
			UpdateContent();
		}

		public ObservableCollection<DoorCommandType> Commands { get; private set; }

		DoorCommandType _selectedCommand;
		public DoorCommandType SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				ControlDoorArguments.DoorCommandType = value;
				OnPropertyChanged(()=>SelectedCommand);
			}
		}
		
		public override void UpdateContent()
		{
			Variable1.Update(ProcedureHelper.GetAllVariables(Procedure, ValueType.Object, ObjectType.ControlDoor, false));
		}

		public override string Description
		{
			get { return ""; }
		}
	}
}
