using System.Collections.Generic;
using FiresecClient;

namespace VideoModule.ViewModels
{
	public static class VlcControlHelper
	{
		public static List<VlcControlViewModel> VlcControlViewModels { get; set; }
		static VlcControlHelper ()
		{
			VlcControlViewModels = new List<VlcControlViewModel>();
			//foreach (var camera in FiresecManager.SystemConfiguration.Cameras)
			//{
			//	var vlcControlViewModel = new VlcControlViewModel();
			//	vlcControlViewModel.RviRTSP = camera.RviRTSP;
			//	VlcControlViewModels.Add(vlcControlViewModel);
			//}
		}
	}
}
