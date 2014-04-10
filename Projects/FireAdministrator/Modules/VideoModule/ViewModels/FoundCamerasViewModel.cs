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
	public class FoundCamerasViewModel : SaveCancelDialogViewModel
	{
		List<CameraViewModel> Cameras { get; set; }
		public FoundCamerasViewModel(List<CameraViewModel> cameras)
		{
			Title = "Автопоиск";
			Cameras = cameras;
			SearchCommand = new RelayCommand(OnSearch);
		}

		private ObservableCollection<FoundCameraViewModel> _foundCameras;
		public ObservableCollection<FoundCameraViewModel> FoundCameras
		{
			get { return _foundCameras; }
			set
			{
				_foundCameras = value;
				OnPropertyChanged(() => FoundCameras);
			}
		}

		private FoundCameraViewModel _selectedFoundCamera;
		public FoundCameraViewModel SelectedFoundCamera
		{
			get { return _selectedFoundCamera; }
			set
			{
				_selectedFoundCamera = value;
				OnPropertyChanged(() => SelectedFoundCamera);
			}
		}

		public RelayCommand SearchCommand { get; private set; }
		void OnSearch()
		{
			var perimeter = SystemPerimeter.Instance;
			FoundCameras = new ObservableCollection<FoundCameraViewModel>();
			perimeter.NewDevice -= OnNewDeviceFound;
			perimeter.NewDevice += OnNewDeviceFound;
			perimeter.StartSearchDevices();
		}

		void OnNewDeviceFound(object sender, SearchDevicesEventArgs e)
		{
			Dispatcher.BeginInvoke(
					DispatcherPriority.Input, new ThreadStart(
						() =>
						{
							var deviceInfoViewModel = e.DeviceSearchInfo;
							if (FoundCameras.Any(model => model.Address == deviceInfoViewModel.IpAddress))
								return;
							var foundCameraViewModel = new FoundCameraViewModel(deviceInfoViewModel);
							foundCameraViewModel.IsAdded = Cameras.Any(x => x.Camera.Address == foundCameraViewModel.Address && x.Camera.Port == foundCameraViewModel.Port);
							FoundCameras.Add(foundCameraViewModel);
						}));
		}
	}
}
