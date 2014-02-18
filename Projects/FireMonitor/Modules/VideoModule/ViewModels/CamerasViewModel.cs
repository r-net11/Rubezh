using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using KeyboardKey = System.Windows.Input.Key;
using Infrustructure.Plans.Events;
using System;
using Infrustructure.Plans.Elements;
using VideoModule.Plans.Designer;
using FiresecAPI.Models;
using System.Collections.Generic;

namespace VideoModule.ViewModels
{
	public class CamerasViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		private bool _lockSelection;
		public CamerasViewModel()
		{
			_lockSelection = false;
			PlayVideoCommand = new RelayCommand(OnPlayVideo, () => SelectedCamera != null);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, () => SelectedCamera != null);
			Initialize();
		}
		
		public void Initialize()
		{
			Cameras = new ObservableCollection<CameraViewModel>();
			foreach (var camera in FiresecManager.SystemConfiguration.Cameras)
			{
				var cameraViewModel = new CameraViewModel(this, camera);
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
				if (!_lockSelection && SelectedCamera != null && SelectedCamera.Camera.PlanElementUIDs.Count > 0)
					ServiceFactory.Events.GetEvent<FindElementEvent>().Publish(SelectedCamera.Camera.PlanElementUIDs);
			}
		}

		public CameraViewModel StartedCamera
		{
			get { return Cameras.FirstOrDefault(x => x.IsNowPlaying); }
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
			foreach (var camera in Cameras)
			{
				if (camera.IsNowPlaying)
					camera.StopVideo();
			}
			if (!IsNowPlaying)
				SelectedCamera.StartVideo();
			IsNowPlaying = !IsNowPlaying;
			OnPropertyChanged("StartedCamera");
			OnPropertyChanged("IsNowPlaying");
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			if (SelectedCamera.Camera.PlanElementUIDs.Count > 0)
				ServiceFactoryBase.Events.GetEvent<FindElementEvent>().Publish(SelectedCamera.Camera.PlanElementUIDs);
		}

		public void Select(Guid cameraUID)
		{
			if (cameraUID != Guid.Empty)
				SelectedCamera = Cameras.FirstOrDefault(item => item.Camera.UID == cameraUID);
		}
	}
}