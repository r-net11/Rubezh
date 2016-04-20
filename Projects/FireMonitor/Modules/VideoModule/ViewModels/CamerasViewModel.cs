using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace VideoModule.ViewModels
{
	public class CamerasViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		public static CamerasViewModel Current { get; private set; }
		List<CameraViewModel> AllCameras { get; set; }
		public CamerasViewModel()
		{
			Current = this;
			Initialize();
		}

		public void Initialize()
		{
			Cameras = new ObservableCollection<CameraViewModel>();
			AllCameras = new List<CameraViewModel>();
			BuildTree();
			SelectedCamera = Cameras.FirstOrDefault();
		}

		public ObservableCollection<CameraViewModel> Cameras { get; private set; }

		CameraViewModel _selectedCamera;
		public CameraViewModel SelectedCamera
		{
			get { return _selectedCamera; }
			set
			{
				_selectedCamera = value;
				if (value != null)
					value.ExpandToThis();
				OnPropertyChanged(() => SelectedCamera);
			}
		}
		public void Select(Guid cameraUid)
		{
			if (cameraUid != Guid.Empty)
				SelectedCamera = AllCameras.FirstOrDefault(x => (x.Camera != null && x.Camera.UID == cameraUid) || (x.RviDevice != null && x.RviDevice.Uid == cameraUid));
		}
		void BuildTree()
		{
			var servers = ClientManager.SystemConfiguration.RviServers;
			foreach (var server in servers)
			{
				var serverViewModel = new CameraViewModel(server);
				foreach (var device in server.RviDevices)
				{
					var deviceViewModel = new CameraViewModel(device);
					foreach (var camera in device.Cameras)
					{
						var cameraViewModel = new CameraViewModel(camera.Name, camera);
						deviceViewModel.AddChild(cameraViewModel);
						AllCameras.Add(cameraViewModel);

					}
					AllCameras.Add(deviceViewModel);
					serverViewModel.AddChild(deviceViewModel);
				}
				Cameras.Add(serverViewModel);
			}
		}
	}
}
