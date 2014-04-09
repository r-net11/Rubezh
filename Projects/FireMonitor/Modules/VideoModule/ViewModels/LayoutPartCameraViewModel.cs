using System;
using System.Linq;
using FiresecAPI.Models;
using FiresecAPI.Models.Layouts;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Video.RVI_VSS;
using Infrastructure.Common.Windows.ViewModels;

namespace VideoModule.ViewModels
{
	public class LayoutPartCameraViewModel : BaseViewModel
	{
		private string ViewName { get; set; }
		public Camera Camera { get; set; }
		public CellPlayerWrap CellPlayerWrap { get; private set; }
		public LayoutPartCameraViewModel(LayoutPartCameraProperties properties)
		{
			CellPlayerWrap = new CellPlayerWrap();
			if (properties != null)
			{
				Camera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault(item => item.UID == properties.SourceUID);
				CameraViewModel = new CameraViewModel(Camera, CellPlayerWrap);
				CameraViewModel.ConnectAndStart();
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