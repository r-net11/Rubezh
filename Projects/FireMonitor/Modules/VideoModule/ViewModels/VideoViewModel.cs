using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using System;
using Infrastructure.Common;
using Infrastructure;
using Infrastructure.Events;

namespace VideoModule.ViewModels
{
	public class VideoViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		public VideoViewModel()
		{
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
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

		public RelayCommand ShowOnPlanCommand { get; private set; }
		private void OnShowOnPlan()
		{
			ServiceFactory.Events.GetEvent<ShowCameraOnPlanEvent>().Publish(SelectedCamera.Camera);
		}
		public bool CanShowOnPlan()
		{
			if (SelectedCamera != null)
			{
				foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
					if (plan.ElementExtensions.OfType<ElementCamera>().Any(x => x.CameraUID == SelectedCamera.Camera.UID))
						return true;
			}
			return false;
		}

		#region ISelectable<Guid> Members

		public void Select(Guid cameraUID)
		{
			if (cameraUID != Guid.Empty)
				SelectedCamera = Cameras.FirstOrDefault(item => item.Camera.UID == cameraUID);
		}

		#endregion
	}
}