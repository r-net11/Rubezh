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
	public class ControlCameraStepViewModel: BaseViewModel, IStepViewModel
	{
		ControlCameraArguments ControlCameraArguments { get; set; }
		public ArithmeticParameterViewModel Variable1 { get; private set; }
		Procedure Procedure { get; set; }

		public ControlCameraStepViewModel(ControlCameraArguments controlCameraArguments, Procedure procedure)
		{
			ControlCameraArguments = controlCameraArguments;
			Procedure = procedure;
			Commands = new ObservableCollection<CameraCommandType>
			{
				CameraCommandType.StartRecord, CameraCommandType.StopRecord
			};
			Variable1 = new ArithmeticParameterViewModel(ControlCameraArguments.Variable1);
			OnPropertyChanged(() => Commands);
			SelectCameraCommand = new RelayCommand(OnSelectCamera);
			UpdateContent();
		}

		public ObservableCollection<CameraCommandType> Commands { get; private set; }

		CameraCommandType _selectedCommand;
		public CameraCommandType SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				ControlCameraArguments.CameraCommandType = value;
				OnPropertyChanged(()=>SelectedCommand);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		CameraViewModel _selectedCamera;
		public CameraViewModel SelectedCamera
		{
			get { return _selectedCamera; }
			set
			{
				_selectedCamera = value;
				Variable1.UidValue = Guid.Empty;
				if (_selectedCamera != null)
				{
					Variable1.UidValue = _selectedCamera.Camera.UID;
				}
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedCamera);
			}
		}
		
		public RelayCommand SelectCameraCommand { get; private set; }
		private void OnSelectCamera()
		{
			var cameraSelectionViewModel = new CameraSelectionViewModel(SelectedCamera != null ? SelectedCamera.Camera : null);
			if (DialogService.ShowModalWindow(cameraSelectionViewModel))
			{
				SelectedCamera = cameraSelectionViewModel.SelectedCamera;
			}
		}

		public void UpdateContent()
		{
			Variable1.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ValueType == ValueType.Object && x.ObjectType == ObjectType.VideoDevice && !x.IsList));
			if (Variable1.UidValue != Guid.Empty)
			{
				var camera = FiresecManager.SystemConfiguration.AllCameras.FirstOrDefault(x => x.UID == Variable1.UidValue);
				SelectedCamera = camera != null ? new CameraViewModel(camera) : null;
				SelectedCommand = ControlCameraArguments.CameraCommandType;
			}
		}

		public string Description
		{
			get { return ""; }
		}
	}
}
