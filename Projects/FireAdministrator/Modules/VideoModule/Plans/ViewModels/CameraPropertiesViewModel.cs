using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VideoModule.ViewModels;

namespace VideoModule.Plans.ViewModels
{
	public class CameraPropertiesViewModel : SaveCancelDialogViewModel
	{
		ElementCamera _elementCamera;
		CamerasViewModel _camerasViewModelForPlanExtension;
		List<CameraViewModel> CamerasForUpdate { get; set; }

		public CameraPropertiesViewModel(ElementCamera elementCamera)
		{
			Title = "Свойства фигуры: Камера";
			_elementCamera = elementCamera;
			_camerasViewModelForPlanExtension = new CamerasViewModel();
			_camerasViewModelForPlanExtension.Initialize();

			Cameras = _camerasViewModelForPlanExtension.Cameras;
			SelectedCamera = _camerasViewModelForPlanExtension.AllCameras.FirstOrDefault(item => item.IsCamera && item.Camera.UID == elementCamera.CameraUID);
			Rotation = elementCamera.Rotation;
		}

		public ObservableCollection<CameraViewModel> Cameras { get; private set; }

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

		private int _rotation = 0;
		public int Rotation
		{
			get { return _rotation; }
			set
			{
				_rotation = value;
				OnPropertyChanged(() => Rotation);
			}
		}

		protected override bool Save()
		{
			PlanExtension.Instance.RewriteItem(_elementCamera, SelectedCamera.Camera);
			_elementCamera.Rotation = Rotation;
			return base.Save();
		}

		protected override bool CanSave()
		{
			if (SelectedCamera == null)
				return false;
			if (!SelectedCamera.IsCamera)
				return false;
			if (SelectedCamera.IsOnPlan && !SelectedCamera.Camera.AllowMultipleVizualization && SelectedCamera.Camera.UID != _elementCamera.CameraUID)
				return false;
			return true;
		}
	}
}