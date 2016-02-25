using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
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
		public bool StatusVisibility { get { return RviServer != null || RviDevice != null; } }
		public CameraViewModel()
		{
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
		}
		public CameraViewModel(string presentationName, Camera camera) : this()
		{
			Camera = camera;
			PresentationName = presentationName;
		}
		public CameraViewModel(RviDevice rviDevice) : this()
		{
			RviDevice = rviDevice;
			Status = rviDevice.Status;
			PresentationName = rviDevice.Name;
			PresentationAddress = rviDevice.Ip;
			rviDevice.StatusChanged += OnDeviceStatusChanged;
		}
		public CameraViewModel(RviServer rviServer) : this()
		{
			RviServer = rviServer;
			Status = rviServer.Status;
			PresentationName = rviServer.PresentationName;
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
		bool CanShowOnPlan()
		{
			return Camera != null && Camera.PlanElementUIDs.Count > 0;
		}
	}
}