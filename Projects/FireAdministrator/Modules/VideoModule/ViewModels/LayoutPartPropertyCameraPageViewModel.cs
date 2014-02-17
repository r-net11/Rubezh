using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Common;
using FiresecAPI.Models.Layouts;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows;
using Microsoft.Win32;
using FiresecAPI.Models;
using FiresecClient;

namespace VideoModule.ViewModels
{
	public class LayoutPartPropertyCameraPageViewModel : LayoutPartPropertyPageViewModel
	{
		private LayoutPartCameraViewModel _layoutPartCameraViewModel;

		public LayoutPartPropertyCameraPageViewModel(LayoutPartCameraViewModel layoutPartCameraViewModel)
		{
			_layoutPartCameraViewModel = layoutPartCameraViewModel;
			Cameras = new ObservableCollection<Camera>(FiresecManager.SystemConfiguration.Cameras);
			CopyProperties();
			UpdateLayoutPart();
		}

		private ObservableCollection<Camera> _cameras;
		public ObservableCollection<Camera> Cameras
		{
			get { return _cameras; }
			set
			{
				_cameras = value;
				OnPropertyChanged(() => Cameras);
			}
		}

		private Camera _selectedCamera;
		public Camera SelectedCamera
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
			var properties = (LayoutPartCameraProperties)_layoutPartCameraViewModel.Properties;
			SelectedCamera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault(item => item.UID == properties.SourceUID);
		}
		public override bool CanSave()
		{
			return true;
		}
		public override bool Save()
		{
			var properties = (LayoutPartCameraProperties)_layoutPartCameraViewModel.Properties;
			if ((SelectedCamera == null && properties.SourceUID != Guid.Empty) || (SelectedCamera != null && properties.SourceUID != SelectedCamera.UID))
			{
				properties.SourceUID = SelectedCamera == null ? Guid.Empty : SelectedCamera.UID;
				UpdateLayoutPart();
				return true;
			}
			return false;
		}

		private void UpdateLayoutPart()
		{
			var properties = (LayoutPartCameraProperties)_layoutPartCameraViewModel.Properties;
			_layoutPartCameraViewModel.CameraTitle = SelectedCamera == null ? _layoutPartCameraViewModel.Title : SelectedCamera.Name;
		}
	}
}