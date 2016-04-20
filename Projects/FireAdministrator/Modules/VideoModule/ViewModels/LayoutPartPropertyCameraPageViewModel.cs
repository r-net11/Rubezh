using Infrastructure.Common.Windows.Services.Layout;
using RubezhAPI.Models.Layouts;
using RubezhClient;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace VideoModule.ViewModels
{
	public class LayoutPartPropertyCameraPageViewModel : LayoutPartPropertyPageViewModel
	{
		private LayoutPartCameraViewModel _layoutPartCameraViewModel;

		public LayoutPartPropertyCameraPageViewModel(LayoutPartCameraViewModel layoutPartCameraViewModel)
		{
			_layoutPartCameraViewModel = layoutPartCameraViewModel;
			Initialize();
		}

		public void Initialize()
		{
			Cameras = new ObservableCollection<CameraViewModel>();
			foreach (var camera in ClientManager.SystemConfiguration.Cameras)
			{
				var cameraViewModel = new CameraViewModel(CamerasViewModel.Current, camera, camera.PresentationName);
				Cameras.Add(cameraViewModel);
			}
			SelectedCamera = Cameras.FirstOrDefault();
		}

		private ObservableCollection<CameraViewModel> _cameras;
		public ObservableCollection<CameraViewModel> Cameras
		{
			get { return _cameras; }
			set
			{
				_cameras = value;
				OnPropertyChanged(() => Cameras);
			}
		}

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

		public override string Header
		{
			get { return "Видеокамера"; }
		}
		public override void CopyProperties()
		{
			var properties = (LayoutPartReferenceProperties)_layoutPartCameraViewModel.Properties;
			var camera = ClientManager.SystemConfiguration.Cameras.FirstOrDefault(item => item.UID == properties.ReferenceUID);
			SelectedCamera = Cameras.FirstOrDefault(x => x.Camera == camera);
		}
		public override bool CanSave()
		{
			return (SelectedCamera != null && SelectedCamera.Camera != null);
		}
		public override bool Save()
		{
			var properties = (LayoutPartReferenceProperties)_layoutPartCameraViewModel.Properties;
			if ((SelectedCamera == null && properties.ReferenceUID != Guid.Empty) || (SelectedCamera != null && properties.ReferenceUID != SelectedCamera.Camera.UID))
			{
				properties.ReferenceUID = SelectedCamera == null ? Guid.Empty : SelectedCamera.Camera.UID;
				if (SelectedCamera == null)
					_layoutPartCameraViewModel.UpdateLayoutPart(null);
				else
					_layoutPartCameraViewModel.UpdateLayoutPart(SelectedCamera.Camera);
				return true;
			}
			return false;
		}
	}
}