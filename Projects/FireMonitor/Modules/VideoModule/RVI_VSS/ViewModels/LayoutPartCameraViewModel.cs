using System;
using System.Linq;
using System.Windows.Media.Imaging;
using FiresecAPI.Models;
using FiresecAPI.Models.Layouts;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using VideoModule.RVI_VSS.ViewModels;

namespace VideoModule.ViewModels
{
	public class LayoutPartCameraViewModel : BaseViewModel
	{
		private string ViewName { get; set; }

		public LayoutPartCameraViewModel(LayoutPartCameraProperties properties)
		{
			if (properties != null)
				Camera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault(item => item.UID == properties.SourceUID);
			InitializeCommand();
		}

		public LayoutPartCameraViewModel(string viewName)
		{
			ViewName = viewName;
			var cameraUID = ClientSettings.MultiLayoutCameraSettings.Dictionary.FirstOrDefault(x => x.Key == viewName).Value;
			if (cameraUID != Guid.Empty)
			{
				Camera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == cameraUID);
				CameraViewModel = new CameraViewModel(Camera);
			}
			InitializeCommand();
		}

		public Camera Camera { get; set; }

		private CameraViewModel _cameraViewModel;
		public CameraViewModel CameraViewModel
		{
			get { return _cameraViewModel; }
			set
			{
				_cameraViewModel = value;
				OnPropertyChanged("CameraViewModel");
			}
		}

		void InitializeCommand()
		{
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			RemoveCommand = new RelayCommand(OnRemove, () => CameraViewModel != null);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			var layoutPartPropertyCameraPageViewModel = new LayoutPartPropertyCameraPageViewModel();
			layoutPartPropertyCameraPageViewModel.SelectedCamera = Camera;
			if (DialogService.ShowModalWindow(layoutPartPropertyCameraPageViewModel))
			{
				Camera = layoutPartPropertyCameraPageViewModel.SelectedCamera;
				if (ClientSettings.MultiLayoutCameraSettings.Dictionary.FirstOrDefault(x => x.Key == ViewName).Value == Camera.UID)
					return;
				ClientSettings.MultiLayoutCameraSettings.Dictionary.Add(ViewName, Camera.UID);
				CameraViewModel = new CameraViewModel(Camera);
				CameraViewModel.StartVideo();
			}
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			ClientSettings.MultiLayoutCameraSettings.Dictionary.Remove(ViewName);
			Camera = null;
			CameraViewModel.StopVideo();
			CameraViewModel = null;
		}
	}
}