using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using VideoModule.ViewModels;
using VideoModule.Plans.Designer;
using System.Collections.ObjectModel;

namespace VideoModule.Plans.ViewModels
{
	public class CameraPropertiesViewModel : SaveCancelDialogViewModel
	{
		private ElementCamera _elementCamera;
		private CamerasViewModel _camerasViewModel;

		public CameraPropertiesViewModel(CamerasViewModel camerasViewModel, ElementCamera elementCamera)
		{
			Title = "Свойства фигуры: Камера";
			_elementCamera = elementCamera;
			_camerasViewModel = camerasViewModel;
			Initialize();
			SelectedCamera = AllCameras.FirstOrDefault(item => item.Camera.UID == elementCamera.CameraUID);
			if (SelectedCamera != null)
				SelectedCamera.ExpandToThis();
		}

		public void Initialize()
		{
			Cameras = new ObservableCollection<CameraViewModel>();
			foreach (var camera in FiresecManager.SystemConfiguration.Cameras)
			{
				var cameraViewModel = new CameraViewModel(_camerasViewModel, camera);
				Cameras.Add(cameraViewModel);
			}
			SelectedCamera = Cameras.FirstOrDefault();
		}

		public ObservableCollection<CameraViewModel> Cameras { get; private set; }
		public List<CameraViewModel> AllCameras
		{
			get
			{
				var cameras = new List<CameraViewModel>();
				foreach (var camera in Cameras)
				{
					cameras.Add(camera);
					foreach (var child in camera.Children)
					{
						cameras.Add(child);
					}
				}
				return cameras;
			}
		}

		private CameraViewModel _selectedCamera;
		public CameraViewModel SelectedCamera
		{
			get { return _selectedCamera; }
			set
			{
				_selectedCamera = value;
				OnPropertyChanged(() => SelectedCamera);
			}
		}

		protected override bool Save()
		{
			Guid cameraUID = _elementCamera.CameraUID;
			Helper.SetCamera(_elementCamera, SelectedCamera.Camera);

			if (cameraUID != _elementCamera.CameraUID)
				Update(cameraUID);
			_camerasViewModel.SelectedCamera = Update(_elementCamera.CameraUID);
			return base.Save();
		}

		protected override bool CanSave()
		{
			if (SelectedCamera.IsDvr)
				return false;
			if (SelectedCamera.IsOnPlan && !SelectedCamera.Camera.AllowMultipleVizualization && SelectedCamera.Camera.UID != _elementCamera.CameraUID)
				return false;
			return true;
		}

		private CameraViewModel Update(Guid cameraUID)
		{
			var cameraViewModel = _camerasViewModel.AllCameras.FirstOrDefault(x => x.Camera.UID == cameraUID);
			if (cameraViewModel != null)
				cameraViewModel.Update();
			return cameraViewModel;
		}
	}
}