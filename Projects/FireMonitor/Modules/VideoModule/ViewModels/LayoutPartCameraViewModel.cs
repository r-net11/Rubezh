using System;
using System.Linq;
using System.Threading;
using Common;
using Entities.DeviceOriented;
using FiresecAPI.Models;
using FiresecAPI.Models.Layouts;
using FiresecClient;
using Infrastructure.Common.Video.RVI_VSS;
using Infrastructure.Common.Windows.ViewModels;

namespace VideoModule.ViewModels
{
	public class LayoutPartCameraViewModel : BaseViewModel
	{
		Camera Camera { get; set; }
		public CellPlayerWrap CellPlayerWrap { get; private set; }
		public LayoutPartCameraViewModel(LayoutPartReferenceProperties properties)
		{
			CellPlayerWrap = new CellPlayerWrap();
			if (properties != null)
			{
				Camera = FiresecManager.SystemConfiguration.AllCameras.FirstOrDefault(item => item.UID == properties.ReferenceUID);
				if (Camera == null)
					return;
				Camera.Status = DeviceStatuses.Connected;
				CameraViewModel = new CameraViewModel(Camera, CellPlayerWrap);
				new Thread(delegate()
				{
					try
					{
						if ((CameraViewModel.Camera != null) && (CameraViewModel.Camera.Ip != null))
						{
							CameraViewModel.Connect();
							CameraViewModel.Start();
						}
					}
					catch (Exception e)
					{
						Logger.Error(e);
					}
				}).Start();
			}
		}

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
	}
}