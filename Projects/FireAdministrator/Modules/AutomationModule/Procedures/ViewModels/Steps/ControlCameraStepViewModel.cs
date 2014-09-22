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
		public ArgumentViewModel CameraParameter { get; private set; }

		public ControlCameraStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ControlCameraArguments = stepViewModel.Step.ControlCameraArguments;
			Commands = ProcedureHelper.GetEnumObs<CameraCommandType>();
			CameraParameter = new ArgumentViewModel(ControlCameraArguments.CameraParameter, stepViewModel.Update);
			CameraParameter.SelectedObjectType = ObjectType.VideoDevice;
			CameraParameter.ExplicitType = ExplicitType.Object;
			SelectedCommand = ControlCameraArguments.CameraCommandType;
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
			CameraParameter.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.Object && x.ObjectType == ObjectType.VideoDevice && !x.IsList));
		}

		public override string Description
		{
			get 
			{ 
				return "Камера: " + CameraParameter.Description + " Команда: " + SelectedCommand.ToDescription(); 
			}
		}
	}
}
