using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using RubezhAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VideoModule.ViewModels
{
	public class CameraViewModel : TreeNodeViewModel<CameraViewModel>
	{
		public Camera Camera { get; set; }
		public RviServer RviServer { get; private set; }
		public RviDevice RviDevice { get; private set; }
		public string PresentationName { get; private set; }
		public string PresentationAddress { get; private set; }
		public RviStatus Status { get; private set; }
		public bool StatusVisibility { get { return RviServer != null || RviDevice != null; } }
		public List<CameraViewModel> VisualCameraViewModels;
		public CameraViewModel()
		{
			VisualCameraViewModels = new List<CameraViewModel>();
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, () => Camera != null && Camera.PlanElementUIDs.Count > 0);
			Status = RviStatus.Unknown;
		}
		public CameraViewModel(string presentationName, Camera camera = null) : this()
		{
			Camera = camera;
			PresentationName = presentationName;
		}
		public CameraViewModel(RviDevice rviDevice) : this()
		{
			RviDevice = rviDevice;
			PresentationName = rviDevice.Name;
			PresentationAddress = rviDevice.Ip;
			rviDevice.StatusChanged += OnDeviceStatusChanged;
		}
		public CameraViewModel(RviServer rviServer) : this()
		{
			RviServer = rviServer;
			PresentationName = rviServer.Name;
			rviServer.StatusChanged += OnServerStatusChanged;
		}
		void OnServerStatusChanged()
		{
			Status = RviServer.Status;
			OnPropertyChanged(() => Status);
		}
		void OnDeviceStatusChanged()
		{
			Status = RviDevice.Status;
			OnPropertyChanged(() => Status);
		}
		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			DialogService.ShowWindow(new CameraDetailsViewModel(Camera));
		}
		bool CanShowProperties()
		{
			return Camera != null;
		}


		public void Update()
		{
			OnPropertyChanged(() => Camera);
			OnPropertyChanged(() => PresentationAddress);
			OnPropertyChanged(() => IsOnPlan);
		}

		public bool IsOnPlan
		{
			get { return Camera.PlanElementUIDs.Count > 0; }
		}

		public void StartAll()
		{
			foreach (var visualCameraViewModel in VisualCameraViewModels)
			{
				visualCameraViewModel.Start(false);
			}
		}

		public void StopAll()
		{
			foreach (var visualCameraViewModel in VisualCameraViewModels)
			{
				visualCameraViewModel.Stop(false);
			}
		}

		public void Start(bool addToRootCamera = true)
		{
			//_cellPlayerWrap.Start(Camera, Camera.ChannelNumber);
			if ((addToRootCamera) && (RootCamera != null))
				RootCamera.VisualCameraViewModels.Add(this);
		}

		public void Stop(bool addToRootCamera = true)
		{
			//_cellPlayerWrap.Stop();
			if ((addToRootCamera) && (RootCamera != null))
				RootCamera.VisualCameraViewModels.Remove(this);
		}

		CameraViewModel RootCamera
		{
			get
			{
				return CamerasViewModel.Current.Cameras.FirstOrDefault(x => x.Camera.Ip == Camera.Ip);
			}
		}
		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			if (Camera != null)
				ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(new List<Guid> { Camera.UID });
		}
		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			ServiceFactoryBase.Events.GetEvent<ShowCameraOnPlanEvent>().Publish(Camera);
		}

	}
}