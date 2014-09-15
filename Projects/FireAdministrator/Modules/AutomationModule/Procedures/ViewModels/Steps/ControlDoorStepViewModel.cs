using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

namespace AutomationModule.ViewModels
{
	public class ControlDoorStepViewModel : BaseStepViewModel
	{
		ControlDoorArguments ControlDoorArguments { get; set; }
		public ArithmeticParameterViewModel Variable1 { get; private set; }

		public ControlDoorStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ControlDoorArguments = stepViewModel.Step.ControlDoorArguments;
			Commands = ProcedureHelper.GetEnumObs<DoorCommandType>();
			Variable1 = new ArithmeticParameterViewModel(ControlDoorArguments.Variable1, stepViewModel.Update);
			Variable1.ObjectType = ObjectType.ControlDoor;
			Variable1.ExplicitType = ExplicitType.Object;
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
			Variable1.Update(ProcedureHelper.GetAllVariables(Procedure, ExplicitType.Object, ObjectType.ControlDoor, false));
		}

		public override string Description
		{
			get
			{
				return "Точка доступа: " + Variable1.Description + " Команда: " + SelectedCommand.ToDescription();
			}
		}
	}
}
