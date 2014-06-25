using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class CameraSelectationViewModel : BaseViewModel
	{
		public CameraSelectationViewModel()
		{
			var cameras = new ObservableCollection<CameraViewModel>();
			foreach (var camera in FiresecManager.SystemConfiguration.Cameras)
			{
				var cameraViewModel = AddCameraInternal(camera, null);
				cameras.Add(cameraViewModel);
			}
			RootCameras = cameras.ToArray();
			//RootCamera = AddCameraInternal(XManager.DeviceConfiguration.RootDevice, null);
		}

		CameraViewModel AddCameraInternal(Camera camera, CameraViewModel parentCameraViewModel)
		{
			var cameraViewModel = new CameraViewModel(camera);
			if (parentCameraViewModel != null)
				parentCameraViewModel.AddChild(cameraViewModel);

			foreach (var childDevice in camera.Children)
				AddCameraInternal(childDevice, cameraViewModel);
			return cameraViewModel;
		}

		//DeviceViewModel _rootDevice;
		//public DeviceViewModel RootDevice
		//{
		//	get { return _rootDevice; }
		//	private set
		//	{
		//		_rootDevice = value;
		//		OnPropertyChanged(() => RootDevice);
		//		OnPropertyChanged(() => RootDevices);
		//	}
		//}
		//public DeviceViewModel[] RootDevices
		//{
		//	get { return new DeviceViewModel[] { RootDevice }; }
		//}

		//DeviceViewModel _selectedDevice;
		//public DeviceViewModel SelectedDevice
		//{
		//	get { return _selectedDevice; }
		//	set
		//	{
		//		_selectedDevice = value;
		//		OnPropertyChanged("SelectedDevice");
		//	}
		//}

		//public CameraSelectationViewModel()
		//{
		//	Cameras = new ObservableCollection<CameraViewModel>();
		//	foreach (var camera in FiresecManager.SystemConfiguration.Cameras)
		//	{
		//		var cameraViewModel = new CameraViewModel(camera);
		//		Cameras.Add(cameraViewModel);
		//	}
		//}

		//public ObservableCollection<CameraViewModel> Cameras { get; private set; }
		//public List<CameraViewModel> AllCameras
		//{
		//	get
		//	{
		//		var cameras = new List<CameraViewModel>();
		//		foreach (var camera in Cameras)
		//		{
		//			cameras.Add(camera);
		//			foreach (var child in camera.Children)
		//			{
		//				cameras.Add(child);
		//			}
		//		}
		//		return cameras;
		//	}
		//}

		//CameraViewModel _rootCamera;
		//public CameraViewModel RootCamera
		//{
		//	get { return _rootCamera; }
		//	private set
		//	{
		//		_rootCamera = value;
		//		OnPropertyChanged(() => RootCamera);
		//		OnPropertyChanged(() => RootCameras);
		//	}
		//}
		public CameraViewModel[] RootCameras { get; private set; }
		//{
		//	get { return new CameraViewModel[] { RootCamera }; }
		//}

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
	}
}