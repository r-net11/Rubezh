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
	public class ControlGKFireZoneStepViewModel: BaseStepViewModel
	{
		ControlGKFireZoneArguments ControlGKFireZoneArguments { get; set; }
		public ArithmeticParameterViewModel Variable1 { get; private set; }

		public ControlGKFireZoneStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ControlGKFireZoneArguments = stepViewModel.Step.ControlGKFireZoneArguments;
			Commands = ProcedureHelper.GetEnumObs<ZoneCommandType>();
			Variable1 = new ArithmeticParameterViewModel(ControlGKFireZoneArguments.Variable1, stepViewModel.Update);
			Variable1.ObjectType = ObjectType.Zone;
			Variable1.ExplicitType = ExplicitType.Object;
			UpdateContent();
		}

		public ObservableCollection<ZoneCommandType> Commands { get; private set; }

		ZoneCommandType _selectedCommand;
		public ZoneCommandType SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				ControlGKFireZoneArguments.ZoneCommandType = value;
				OnPropertyChanged(()=>SelectedCommand);
			}
		}

		public override void UpdateContent()
		{
			Variable1.Update(ProcedureHelper.GetAllVariables(Procedure, ExplicitType.Object, ObjectType.Zone, false));
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
