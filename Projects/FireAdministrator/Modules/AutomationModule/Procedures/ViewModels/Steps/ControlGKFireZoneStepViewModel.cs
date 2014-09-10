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
	public class ControlGKFireZoneStepViewModel: BaseStepViewModel
	{
		ControlGKFireZoneArguments ControlGKFireZoneArguments { get; set; }
		Procedure Procedure { get; set; }
		public ArithmeticParameterViewModel Variable1 { get; private set; }

		public ControlGKFireZoneStepViewModel(ControlGKFireZoneArguments controlGKFireZoneArguments, Procedure procedure, Action updateDescriptionHandler)
			: base(updateDescriptionHandler)
		{
			ControlGKFireZoneArguments = controlGKFireZoneArguments;
			Procedure = procedure;
			Commands = ProcedureHelper.GetEnumObs<ZoneCommandType>();
			Variable1 = new ArithmeticParameterViewModel(ControlGKFireZoneArguments.Variable1);
			Variable1.ObjectType = ObjectType.Zone;
			Variable1.ValueType = ValueType.Object;
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
			Variable1.Update(ProcedureHelper.GetAllVariables(Procedure, ValueType.Object, ObjectType.Zone, false));
		}

		public override string Description
		{
			get { return ""; }
		}
	}
}
