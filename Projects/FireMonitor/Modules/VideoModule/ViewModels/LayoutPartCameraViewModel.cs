using System.Linq;
using RubezhAPI.Models;
using RubezhAPI.Models.Layouts;
using RubezhClient;
using Infrastructure.Common.Windows.ViewModels;
using RviClient;
using System.Net;

namespace VideoModule.ViewModels
{
	public class LayoutPartCameraViewModel : BaseViewModel
	{
		public Camera Camera { get; private set; }
		public string RviRTSP { get; private set; }

		public LayoutPartCameraViewModel()
		{
		}
		public LayoutPartCameraViewModel(LayoutPartReferenceProperties properties)
		{
			if (properties != null)
			{
				Camera = ClientManager.SystemConfiguration.Cameras.FirstOrDefault(item => item.UID == properties.ReferenceUID);
				if (Camera != null)
					RviRTSP = Camera.RviRTSP;
			}
		}
		public bool PrepareToTranslation(out IPEndPoint ipEndPoint, out int vendorId)
		{
			return RviClientHelper.PrepareToTranslation(ClientManager.SystemConfiguration, Camera, out ipEndPoint, out vendorId);
		}
	}
}