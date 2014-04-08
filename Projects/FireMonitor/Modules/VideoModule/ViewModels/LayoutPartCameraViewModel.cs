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

		public LayoutPartCameraViewModel(LayoutPartCameraProperties properties)
		{
			if (properties != null)
				Camera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault(item => item.UID == properties.SourceUID);
		}

		public LayoutPartCameraViewModel(string viewName)
		{
			ViewName = viewName;
			var cameraUID = ClientSettings.MultiLayoutCameraSettings.Dictionary.FirstOrDefault(x => x.Key == viewName).Value;
			if (cameraUID != Guid.Empty)
			{
				Camera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == cameraUID);
				CameraViewModel = new CameraViewModel(Camera, new CellPlayerWrap());
			}
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
	}
}