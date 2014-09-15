using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

namespace AutomationModule.ViewModels
{
	public class ControlSKDZoneStepViewModel: BaseStepViewModel
	{
		ControlSKDZoneArguments ControlSKDZoneArguments { get; set; }
		public ArithmeticParameterViewModel Variable1 { get; private set; }

		public ControlSKDZoneStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ControlSKDZoneArguments = stepViewModel.Step.ControlSKDZoneArguments;
			Commands = ProcedureHelper.GetEnumObs<SKDZoneCommandType>();
			Variable1 = new ArithmeticParameterViewModel(ControlSKDZoneArguments.Variable1, stepViewModel.Update);
			Variable1.ObjectType = ObjectType.SKDZone;
			Variable1.ExplicitType = ExplicitType.Object;
			UpdateContent();
		}

		public ObservableCollection<SKDZoneCommandType> Commands { get; private set; }
		SKDZoneCommandType _selectedCommand;
		public SKDZoneCommandType SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				ControlSKDZoneArguments.SKDZoneCommandType = value;
				OnPropertyChanged(()=>SelectedCommand);
			}
		}

		public override void UpdateContent()
		{
			Variable1.Update(ProcedureHelper.GetAllVariables(Procedure, ExplicitType.Object, ObjectType.SKDZone, false));
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
