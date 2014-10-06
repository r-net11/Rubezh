using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;

namespace AutomationModule.ViewModels
{
	public class CameraSelectionViewModel : SaveCancelDialogViewModel
	{
		public CameraSelectionViewModel(Camera camera)
		{
			Title = "Выбор видеоустройства";
			var cameras = new ObservableCollection<CameraViewModel>();
			foreach (var cam in FiresecManager.SystemConfiguration.Cameras)
			{
				var cameraViewModel = AddCameraInternal(cam, null);
				cameras.Add(cameraViewModel);
			}
			RootCameras = cameras.ToArray();

			if (camera != null)
				SelectedCamera = AllCameras.FirstOrDefault(x => x.Camera.UID == camera.UID);
			if (SelectedCamera == null)
				SelectedCamera = AllCameras.FirstOrDefault();
			if (SelectedCamera != null)
				SelectedCamera.ExpandToThis();
		}

		CameraViewModel AddCameraInternal(Camera camera, CameraViewModel parentCameraViewModel)
		{
			var cameraViewModel = new CameraViewModel(camera);
			if ((parentCameraViewModel != null) && (parentCameraViewModel.Camera.CameraType == CameraType.Dvr))
				parentCameraViewModel.AddChild(cameraViewModel);

			foreach (var childDevice in camera.Children)
				AddCameraInternal(childDevice, cameraViewModel);
			return cameraViewModel;
		}

		public CameraViewModel[] RootCameras { get; private set; }

		CameraViewModel _selectedCamera;
		public CameraViewModel SelectedCamera
		{
			get { return _selectedCamera; }
			set
			{
				_selectedCamera = value;
				OnPropertyChanged(() => SelectedCamera);
			}
		}

		List<CameraViewModel> AllCameras 
		{ 
			get
			{
				var cameras = new List<CameraViewModel>();
				foreach (var rootCamera in RootCameras)
				{
					cameras.AddRange(rootCamera.GetAllChildren());
				}
				return cameras;
			}
		}

		protected override bool CanSave()
		{
			return ((SelectedCamera == null) || (!SelectedCamera.IsDvr));
		}
	}
}