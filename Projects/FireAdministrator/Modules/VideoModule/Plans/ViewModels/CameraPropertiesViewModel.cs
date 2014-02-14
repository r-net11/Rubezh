using System;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using VideoModule.ViewModels;
using VideoModule.Plans.Designer;
using System.Collections.ObjectModel;
using FiresecClient;

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

			SelectedCamera = Cameras.FirstOrDefault(item => item.Camera.UID == elementCamera.CameraUID);
		}

		public ObservableCollection<CameraViewModel> Cameras
		{
			get { return _camerasViewModel.Cameras; }
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
		private CameraViewModel Update(Guid cameraUID)
		{
			var cameraViewModel = _camerasViewModel.Cameras.FirstOrDefault(x => x.Camera.UID == cameraUID);
			if (cameraViewModel != null)
				cameraViewModel.Update();
			return cameraViewModel;
		}
	}
}