using System;
using System.Collections.Generic;
using Infrastructure.Common.Windows.ViewModels;

namespace VideoModule.ViewModels
{
	public class LayoutMultiCameraViewModel : BaseViewModel
	{
		public List<CameraViewModel> CameraViewModels { get; private set; }

		public LayoutMultiCameraViewModel(List<CameraViewModel> cameraViewModels)
		{
			CameraViewModels = cameraViewModels;
		}
	}
}