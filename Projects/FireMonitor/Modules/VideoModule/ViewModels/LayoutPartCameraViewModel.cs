using System.Linq;
using FiresecAPI.Models;
using FiresecAPI.Models.Layouts;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace VideoModule.ViewModels
{
	public class LayoutPartCameraViewModel : BaseViewModel
	{
		public CameraViewModel CameraViewModel { get; set; }
		public Camera Camera { get; set; }

		public LayoutPartCameraViewModel(Camera camera)
		{
			Camera = camera;
			CameraViewModel = new CameraViewModel(Camera);
		}

		public LayoutPartCameraViewModel(LayoutPartCameraProperties properties)
		{
			if (properties != null)
			{
				Camera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault(item => item.UID == properties.SourceUID);
				CameraViewModel = new CameraViewModel(Camera);
			}
		}
	}
}