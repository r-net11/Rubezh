using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.ViewModels;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using RubezhClient;
using System.Collections.ObjectModel;
using System.Linq;
using VideoModule.ViewModels;

namespace VideoModule.Plans.ViewModels
{
	public class CameraPropertiesViewModel : SaveCancelDialogViewModel
	{
		ElementCamera _elementCamera;
		public PositionSettingsViewModel PositionSettingsViewModel { get; private set; }
		public CameraPropertiesViewModel(ElementCamera element, CommonDesignerCanvas designerCanvas)
		{
			Title = "Свойства фигуры: Камера";
			_elementCamera = element;
			PositionSettingsViewModel = new PositionSettingsViewModel(element as ElementBase, designerCanvas);

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
			PositionSettingsViewModel.SavePosition();
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