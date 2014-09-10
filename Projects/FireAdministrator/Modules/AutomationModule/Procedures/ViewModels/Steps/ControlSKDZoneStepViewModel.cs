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
using ValueType = FiresecAPI.Automation.ValueType;

namespace AutomationModule.ViewModels
{
	public class ControlSKDZoneStepViewModel: BaseStepViewModel
	{
		ControlSKDZoneArguments ControlSKDZoneArguments { get; set; }
		Procedure Procedure { get; set; }
		public ArithmeticParameterViewModel Variable1 { get; private set; }

		public ControlSKDZoneStepViewModel(ControlSKDZoneArguments controlSKDZoneArguments, Procedure procedure, Action updateDescriptionHandler)
			: base(updateDescriptionHandler)
		{
			ControlSKDZoneArguments = controlSKDZoneArguments;
			Procedure = procedure;
			Commands = ProcedureHelper.GetEnumObs<SKDZoneCommandType>();
			Variable1 = new ArithmeticParameterViewModel(ControlSKDZoneArguments.Variable1);
			Variable1.ObjectType = ObjectType.SKDZone;
			Variable1.ValueType = ValueType.Object;
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
			Variable1.Update(ProcedureHelper.GetAllVariables(Procedure, ValueType.Object, ObjectType.SKDZone, false));
		}

		public override string Description
		{
			get { return ""; }
		}
	}
}
