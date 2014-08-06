using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ControlCameraStepViewModel: BaseViewModel, IStepViewModel
	{
		ControlCameraArguments ControlCameraArguments { get; set; }
		public ControlCameraStepViewModel(ControlCameraArguments controlCameraArguments)
		{
			ControlCameraArguments = controlCameraArguments;
			Commands = new ObservableCollection<CameraCommandType>
			{
				CameraCommandType.StartRecord, CameraCommandType.StopRecord
			};
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
				ControlCameraArguments.CameraUid = Guid.Empty;
				if (_selectedCamera != null)
				{
					ControlCameraArguments.CameraUid = _selectedCamera.Camera.UID;
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
			if (ControlCameraArguments.CameraUid != Guid.Empty)
			{
				var camera = FiresecManager.SystemConfiguration.AllCameras.FirstOrDefault(x => x.UID == ControlCameraArguments.CameraUid);
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
