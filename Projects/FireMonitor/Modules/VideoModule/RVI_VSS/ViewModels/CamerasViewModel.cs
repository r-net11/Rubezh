using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Video.RVI_VSS;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;

namespace VideoModule.RVI_VSS.ViewModels
{
	public class CamerasViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		public CamerasViewModel()
		{
			PlayVideoCommand = new RelayCommand(OnPlayVideo, () => SelectedCamera != null);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, () => SelectedCamera != null && SelectedCamera.Camera.PlanElementUIDs.Count > 0);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, () => SelectedCamera != null);
			Initialize();
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			DialogService.ShowWindow(new CameraDetailsViewModel(SelectedCamera.Camera));
		}

		public void Initialize()
		{
			Cameras = new ObservableCollection<CameraViewModel>();
			foreach (var camera in FiresecManager.SystemConfiguration.Cameras)
			{
				var cameraViewModel = new CameraViewModel(camera, new CellPlayerWrap());
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
				OnPropertyChanged(() => SelectedCamera);
			}
		}

		private bool _isNowPlaying;

		public bool IsNowPlaying
		{
			get { return _isNowPlaying; }
			set
			{
				_isNowPlaying = value;
				OnPropertyChanged("IsNowPlaying");
			}
		}

		public RelayCommand PlayVideoCommand { get; private set; }
		void OnPlayVideo()
		{
			if (!IsNowPlaying)
				SelectedCamera.StartVideo();
			IsNowPlaying = !IsNowPlaying;
			OnPropertyChanged("StartedCamera");
			OnPropertyChanged("IsNowPlaying");
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			ServiceFactoryBase.Events.GetEvent<ShowCameraOnPlanEvent>().Publish(SelectedCamera.Camera);
		}

		public void Select(Guid cameraUID)
		{
			if (cameraUID != Guid.Empty)
				SelectedCamera = Cameras.FirstOrDefault(item => item.Camera.UID == cameraUID);
		}
	}
}
