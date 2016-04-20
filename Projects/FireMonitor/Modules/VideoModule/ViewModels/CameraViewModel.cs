using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Services;
using Infrastructure.Common.Windows.TreeList;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Events;
using RubezhAPI.Models;
using System;
using System.Collections.Generic;

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
		public bool IsOnGuard { get; private set; }
		public bool IsRecordOnline { get; private set; }
		public string ImageSource { get; private set; }
		public CameraViewModel()
		{
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ShowPropertiesOrPlanCommand = new RelayCommand(OnShowOrPlanProperties, CanShowProperties);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
		}
		public CameraViewModel(string presentationName, Camera camera) : this()
		{
			Camera = camera;
			Status = camera.Status;
			IsOnGuard = camera.IsOnGuard;
			IsRecordOnline = camera.IsRecordOnline;
			PresentationName = presentationName;
			camera.StatusChanged += OnCameraStatusChanged;
			ImageSource = "/Controls;component/RviDevicesIcons/Camera.png";
		}
		public CameraViewModel(RviDevice rviDevice) : this()
		{
			RviDevice = rviDevice;
			Status = rviDevice.Status;
			PresentationName = rviDevice.Name;
			PresentationAddress = rviDevice.Ip;
			rviDevice.StatusChanged += OnDeviceStatusChanged;
			ImageSource = "/Controls;component/RviDevicesIcons/Device.png";
		}
		public CameraViewModel(RviServer rviServer) : this()
		{
			RviServer = rviServer;
			Status = rviServer.Status;
			PresentationName = rviServer.PresentationName;
			rviServer.StatusChanged += OnServerStatusChanged;
			ImageSource = "/Controls;component/RviDevicesIcons/Server.png";
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
		void OnCameraStatusChanged()
		{
			Status = Camera.Status;
			IsOnGuard = Camera.IsOnGuard;
			IsRecordOnline = Camera.IsRecordOnline;
			OnPropertyChanged(() => Status);
			OnPropertyChanged(() => IsOnGuard);
			OnPropertyChanged(() => IsRecordOnline);
		}
		public RelayCommand ShowPropertiesOrPlanCommand { get; private set; }
		void OnShowOrPlanProperties()
		{
			if (ShowOnPlanHelper.ShowObjectOnPlan(Camera))
			DialogService.ShowWindow(new CameraDetailsViewModel(Camera));
		}
		bool CanShowProperties()
		{
			return Camera != null;
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
			if (Camera != null)
			ShowOnPlanHelper.ShowObjectOnPlan(Camera);
		}
		public bool CanShowOnPlan()
		{
			return Camera!= null && ShowOnPlanHelper.CanShowOnPlan(Camera);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }

		void OnShowProperties()
		{
			DialogService.ShowWindow(new CameraDetailsViewModel(Camera));
		}

	}
}