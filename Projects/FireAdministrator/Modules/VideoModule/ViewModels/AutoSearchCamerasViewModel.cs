using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using Entities.DeviceOriented;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace VideoModule.ViewModels
{
	public class AutoSearchCamerasViewModel : SaveCancelDialogViewModel
	{
		List<CameraViewModel> Cameras { get; set; }
		public AutoSearchCamerasViewModel(List<CameraViewModel> cameras)
		{
			Title = "Автопоиск";
			Cameras = cameras;
			OnSearch();
		}

		private ObservableCollection<AutoSearchCameraViewModel> _autoSearchCameras;
		public ObservableCollection<AutoSearchCameraViewModel> AutoSearchCameras
		{
			get { return _autoSearchCameras; }
			set
			{
				_autoSearchCameras = value;
				OnPropertyChanged(() => AutoSearchCameras);
			}
		}

		private AutoSearchCameraViewModel _selectedAutoSearchCamera;
		public AutoSearchCameraViewModel SelectedAutoSearchCamera
		{
			get { return _selectedAutoSearchCamera; }
			set
			{
				_selectedAutoSearchCamera = value;
				OnPropertyChanged(() => SelectedAutoSearchCamera);
			}
		}

		public RelayCommand SearchCommand { get; private set; }
		void OnSearch()
		{
			AutoSearchCameras = new ObservableCollection<AutoSearchCameraViewModel>();
			DeviceManager.Instance.NewDevice -= OnNewDeviceFound;
			DeviceManager.Instance.NewDevice += OnNewDeviceFound;
			DeviceManager.Instance.StartSearchDevices();
		}

		public override bool OnClosing(bool isCanceled)
		{
			DeviceManager.Instance.StopSearchDevices();
			return base.OnClosing(isCanceled);
		}

		void OnNewDeviceFound(object sender, SearchDevicesEventArgs e)
		{
			Dispatcher.BeginInvoke(
					DispatcherPriority.Input, new ThreadStart(
						() =>
						{
							var deviceInfoViewModel = e.DeviceSearchInfo;
							if (AutoSearchCameras.Any(x => x.DeviceSearchInfo.IpAddress == deviceInfoViewModel.IpAddress))
								return;
							var autoSearchCameraViewModel = new AutoSearchCameraViewModel(deviceInfoViewModel);
							autoSearchCameraViewModel.IsAdded = Cameras.Any(x => x.Camera.Address == autoSearchCameraViewModel.DeviceSearchInfo.IpAddress && x.Camera.Port == autoSearchCameraViewModel.DeviceSearchInfo.Port);
							AutoSearchCameras.Add(autoSearchCameraViewModel);
						}));
		}
	}
}
