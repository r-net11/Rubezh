using System.Linq;
using FiresecAPI.Models;
using FiresecAPI.Models.Layouts;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace VideoModule.ViewModels
{
	public class LayoutPartCameraViewModel : BaseViewModel
	{
		public LayoutPartCameraViewModel(LayoutPartCameraProperties properties)
		{
			if (properties != null)
				Camera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault(item => item.UID == properties.SourceUID);
		}

		public Camera Camera { get; private set; }
	}
}