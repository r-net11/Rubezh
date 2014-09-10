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
	public class ControlCameraStepViewModel: BaseStepViewModel
	{
		ControlCameraArguments ControlCameraArguments { get; set; }
		public ArithmeticParameterViewModel Variable1 { get; private set; }
		Procedure Procedure { get; set; }

		public ControlCameraStepViewModel(ControlCameraArguments controlCameraArguments, Procedure procedure, Action updateDescriptionHandler)
			: base(updateDescriptionHandler)
		{
			ControlCameraArguments = controlCameraArguments;
			Procedure = procedure;
			Commands = ProcedureHelper.GetEnumObs<CameraCommandType>();
			Variable1 = new ArithmeticParameterViewModel(ControlCameraArguments.Variable1);
			Variable1.ObjectType = ObjectType.VideoDevice;
			Variable1.ValueType = ValueType.Object;
			UpdateContent();
		}

		public ObservableCollection<CameraCommandType> Commands { get; private set; }
		public CameraCommandType SelectedCommand
		{
			get { return ControlCameraArguments.CameraCommandType; }
			set
			{
				ControlCameraArguments.CameraCommandType = value;
				OnPropertyChanged(()=>SelectedCommand);
			}
		}

		public override void UpdateContent()
		{
			Variable1.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ValueType == ValueType.Object && x.ObjectType == ObjectType.VideoDevice && !x.IsList));
		}

		public override string Description
		{
			get { return ""; }
		}
	}
}
