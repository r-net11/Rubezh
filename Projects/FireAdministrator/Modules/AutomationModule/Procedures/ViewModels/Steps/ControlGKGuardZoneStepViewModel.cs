using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ValueType = FiresecAPI.Automation.ValueType;

namespace AutomationModule.ViewModels
{
	public class ControlGKGuardZoneStepViewModel: BaseStepViewModel
	{
		ControlGKGuardZoneArguments ControlGKGuardZoneArguments { get; set; }
		Procedure Procedure { get; set; }
		public ArithmeticParameterViewModel Variable1 { get; private set; }

		public ControlGKGuardZoneStepViewModel(ControlGKGuardZoneArguments controlGKGuardZoneArguments, Procedure procedure, Action updateDescriptionHandler)
			: base(updateDescriptionHandler)
		{
			ControlGKGuardZoneArguments = controlGKGuardZoneArguments;
			Procedure = procedure;
			Commands = ProcedureHelper.GetEnumObs<GuardZoneCommandType>();
			Variable1 = new ArithmeticParameterViewModel(ControlGKGuardZoneArguments.Variable1);
			Variable1.ObjectType = ObjectType.GuardZone;
			Variable1.ValueType = ValueType.Object;
			UpdateContent();
		}

		public ObservableCollection<GuardZoneCommandType> Commands { get; private set; }
		GuardZoneCommandType _selectedCommand;
		public GuardZoneCommandType SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				ControlGKGuardZoneArguments.GuardZoneCommandType = value;
				OnPropertyChanged(()=>SelectedCommand);
			}
		}
	
		public override void UpdateContent()
		{
			Variable1.Update(ProcedureHelper.GetAllVariables(Procedure, ValueType.Object, ObjectType.GuardZone, false));
		}

		public override string Description
		{
			get { return ""; }
		}
	}
}
