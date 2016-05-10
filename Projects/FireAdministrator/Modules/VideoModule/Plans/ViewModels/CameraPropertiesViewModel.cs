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
		const int _sensivityFactor = 100;
		ElementCamera _elementCamera;
		CamerasViewModel _camerasViewModelForPlanExtension;
		public CameraPropertiesViewModel(ElementCamera element)
		{
			Title = "Свойства фигуры: Камера";
			_elementCamera = element;
			Left = (int)(_elementCamera.Left * _sensivityFactor);
			Top = (int)(_elementCamera.Top * _sensivityFactor);
			_camerasViewModelForPlanExtension = new CamerasViewModel();
			_camerasViewModelForPlanExtension.Initialize();

			Cameras = _camerasViewModelForPlanExtension.Cameras;
			SelectedCamera = _camerasViewModelForPlanExtension.AllCameras.FirstOrDefault(item => item.IsCamera && item.Camera.UID == element.CameraUID);
			Rotation = element.Rotation;
		}

		int _left;
		public int Left
		{
			get { return _left; }
			set
			{
				_left = value;
				OnPropertyChanged(() => Left);
			}
		}
		int _top;
		public int Top
		{
			get { return _top; }
			set
			{
				_top = value;
				OnPropertyChanged(() => Top);
			}
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
			_elementCamera.Left = (double)Left / _sensivityFactor;
			_elementCamera.Top = (double)Top / _sensivityFactor;
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

		public override void OnClosed()
		{
			Unsubscribe(Cameras);
			base.OnClosed();
		}
		void Unsubscribe(IEnumerable<CameraViewModel> cameras)
		{
			foreach (var camera in cameras)
			{
				camera.UnsubscribeEvents();
				Unsubscribe(camera.Children);
			}
		}
	}
}