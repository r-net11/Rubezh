using System.Collections.Generic;
using System.Linq;
using System.Net;
using StrazhAPI.Models;
using StrazhAPI.Models.Layouts;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using RviClient;

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
				Camera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault(item => item.UID == properties.ReferenceUID);
				if (Camera != null)
					RviRTSP = Camera.RviRTSP;
			}
		}

		public bool PrepareToTranslation(out IPEndPoint ipEndPoint, out int vendorId)
		{
			return RviClientHelper.PrepareToTranslation(FiresecManager.SystemConfiguration, Camera, out ipEndPoint, out vendorId);
		}
	}
}