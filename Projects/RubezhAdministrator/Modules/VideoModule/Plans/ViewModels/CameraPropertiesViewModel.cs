using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Models;
using RubezhClient;
using System.Collections.ObjectModel;
using System.Linq;
using VideoModule.ViewModels;

namespace VideoModule.Plans.ViewModels
{
	public class CameraPropertiesViewModel : SaveCancelDialogViewModel
	{
		const int _sensivityFactor = 100;
		ElementCamera _elementCamera;
		public CameraPropertiesViewModel(ElementCamera element)
		{
			Title = "Свойства фигуры: Камера";
			_elementCamera = element;
			Left = (int)(_elementCamera.Left * _sensivityFactor);
			Top = (int)(_elementCamera.Top * _sensivityFactor);

			Initialize();
			Rotation = element.Rotation;
		}

		void Initialize()
		{
			Cameras = new ObservableCollection<SimpleCameraViewModel>();
			var servers = ClientManager.SystemConfiguration.RviServers;
			foreach (var server in servers)
			{
				var serverViewModel = new SimpleCameraViewModel(server);
				foreach (var device in server.RviDevices)
				{
					var deviceViewModel = new SimpleCameraViewModel(device);
					foreach (var camera in device.Cameras)
					{
						var cameraViewModel = new SimpleCameraViewModel(camera);
						deviceViewModel.AddChild(cameraViewModel);
					}
					serverViewModel.AddChild(deviceViewModel);
				}
				Cameras.Add(serverViewModel);
				SelectedCamera = Cameras.FirstOrDefault();
			}
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
		public ObservableCollection<SimpleCameraViewModel> Cameras { get; private set; }

		SimpleCameraViewModel _selectedCamera;
		public SimpleCameraViewModel SelectedCamera
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
	}
}