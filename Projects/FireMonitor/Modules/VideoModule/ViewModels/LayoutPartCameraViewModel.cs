using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI.Models;
using RubezhAPI.Models.Layouts;
using RubezhClient;
using RviClient;
using System.Linq;
using System.Net;

namespace VideoModule.ViewModels
{
	public class LayoutPartCameraViewModel : BaseViewModel
	{
		public Camera Camera { get; set; }
		public LayoutPartCameraViewModel()
		{
		}
		public LayoutPartCameraViewModel(LayoutPartReferenceProperties properties)
		{
			if (properties != null)
			{
				Camera = ClientManager.SystemConfiguration.Cameras.FirstOrDefault(item => item.UID == properties.ReferenceUID);
			}
		}
		public bool PrepareToTranslation(out IPEndPoint ipEndPoint, out int vendorId)
		{
			if (Camera != null)
			{
				return RviClientHelper.PrepareToTranslation(ClientManager.SystemConfiguration.RviSettings, Camera.SelectedRviStream, out ipEndPoint, out vendorId);
			}
			ipEndPoint = null;
			vendorId = int.MinValue;
			return false;
		}
	}
}