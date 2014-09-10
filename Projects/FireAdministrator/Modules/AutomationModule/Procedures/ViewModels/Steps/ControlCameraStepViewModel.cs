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
	public class ControlCameraStepViewModel: BaseStepViewModel
	{
		ControlCameraArguments ControlCameraArguments { get; set; }
		public ArithmeticParameterViewModel Variable1 { get; private set; }

		public ControlCameraStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ControlCameraArguments = stepViewModel.Step.ControlCameraArguments;
			Commands = ProcedureHelper.GetEnumObs<CameraCommandType>();
			Variable1 = new ArithmeticParameterViewModel(ControlCameraArguments.Variable1, stepViewModel.Update);
			Variable1.ObjectType = ObjectType.VideoDevice;
			Variable1.ExplicitType = ExplicitType.Object;
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
			Variable1.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.Object && x.ObjectType == ObjectType.VideoDevice && !x.IsList));
		}

		public override string Description
		{
			get 
			{ 
				return "Камера: " + Variable1.Description + "Команда: " + SelectedCommand.ToDescription(); 
			}
		}
	}
}
