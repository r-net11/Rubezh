using System.Collections.Generic;
using RubezhClient;

namespace VideoModule.ViewModels
{
	public static class LayoutPartCameraHelper
	{
		public static List<LayoutPartCameraViewModel> LayoutPartCameraViewModels { get; set; }
		static LayoutPartCameraHelper ()
		{
			LayoutPartCameraViewModels = new List<LayoutPartCameraViewModel>();
			foreach (var camera in ClientManager.SystemConfiguration.Cameras)
			{
				var layoutPartCameraViewModel = new LayoutPartCameraViewModel();
				layoutPartCameraViewModel.Camera = camera;
				LayoutPartCameraViewModels.Add(layoutPartCameraViewModel);
			}
		}
	}
}
