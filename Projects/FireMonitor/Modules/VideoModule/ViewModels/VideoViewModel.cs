using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using FiresecClient;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace VideoModule.ViewModels
{
	public class VideoViewModel : ViewPartViewModel
	{
		public VideoViewModel()
		{
		}

		public void Initialize()
		{
			Cameras = new ObservableCollection<CameraViewModel>();

			if (FiresecManager.SystemConfiguration.Cameras == null)
				FiresecManager.SystemConfiguration.Cameras = new List<Camera>();

			foreach (var camera in FiresecManager.SystemConfiguration.Cameras)
			{
				var cameraViewModel = new CameraViewModel(camera);
				Cameras.Add(cameraViewModel);
			}
			SelectedCamera = Cameras.FirstOrDefault();
		}

		ObservableCollection<CameraViewModel> _cameras;
		public ObservableCollection<CameraViewModel> Cameras
		{
			get { return _cameras; }
			set
			{
				_cameras = value;
				OnPropertyChanged("Cameras");
			}
		}

		CameraViewModel _selectedCamera;
		public CameraViewModel SelectedCamera
		{
			get { return _selectedCamera; }
			set
			{
				_selectedCamera = value;
				OnPropertyChanged("SelectedCamera");
			}
		}
	}
}