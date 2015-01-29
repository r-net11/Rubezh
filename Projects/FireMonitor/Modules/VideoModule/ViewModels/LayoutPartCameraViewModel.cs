using System;
using System.Linq;
using System.Threading;
using Common;
using Entities.DeviceOriented;
using FiresecAPI.Models;
using FiresecAPI.Models.Layouts;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace VideoModule.ViewModels
{
	public class LayoutPartCameraViewModel : BaseViewModel
	{
		public Camera Camera { get; private set; }
		public LayoutPartCameraViewModel(LayoutPartReferenceProperties properties)
		{
			if (properties != null)
			{
				Camera = FiresecManager.SystemConfiguration.AllCameras.FirstOrDefault(item => item.UID == properties.ReferenceUID);
				if (Camera == null)
					return;
				Camera.Status = DeviceStatuses.Connected;
			}
		}

		CameraViewModel _cameraViewModel;
		public CameraViewModel CameraViewModel
		{
			get { return _cameraViewModel; }
			set
			{
				_cameraViewModel = value;
				OnPropertyChanged(() => CameraViewModel);
			}
		}
	}
}