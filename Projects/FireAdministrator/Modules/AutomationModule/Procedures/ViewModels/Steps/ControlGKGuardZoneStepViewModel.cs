using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

namespace AutomationModule.ViewModels
{
	public class ControlGKGuardZoneStepViewModel: BaseStepViewModel
	{
		ControlGKGuardZoneArguments ControlGKGuardZoneArguments { get; set; }
		public ArithmeticParameterViewModel Variable1 { get; private set; }

		public ControlGKGuardZoneStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ControlGKGuardZoneArguments = stepViewModel.Step.ControlGKGuardZoneArguments;
			Commands = ProcedureHelper.GetEnumObs<GuardZoneCommandType>();
			Variable1 = new ArithmeticParameterViewModel(ControlGKGuardZoneArguments.Variable1, stepViewModel.Update);
			Variable1.ObjectType = ObjectType.GuardZone;
			Variable1.ExplicitType = ExplicitType.Object;
			SelectedCommand = ControlGKGuardZoneArguments.GuardZoneCommandType;
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
			Variable1.Update(ProcedureHelper.GetAllVariables(Procedure, ExplicitType.Object, ObjectType.GuardZone, false));
		}

		public override string Description
		{
			get
			{
				return "Зона: " + Variable1.Description + " Команда: " + SelectedCommand.ToDescription();
			}
		}
	}
}
