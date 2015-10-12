using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecAPI.Models.Layouts;
using RubezhClient;
using Infrastructure.Common.Windows.ViewModels;

namespace VideoModule.ViewModels
{
	public class LayoutPartCameraViewModel : BaseViewModel
	{
		public Camera Camera { get; private set; }
		public string RviRTSP { get; private set; }

		public LayoutPartCameraViewModel(LayoutPartReferenceProperties properties)
		{
			if (properties != null)
			{
				Camera = ClientManager.SystemConfiguration.Cameras.FirstOrDefault(item => item.UID == properties.ReferenceUID);
				if (Camera != null)
					RviRTSP = Camera.RviRTSP;
			}
		}
	}
}