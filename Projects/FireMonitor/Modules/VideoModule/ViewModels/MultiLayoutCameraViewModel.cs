using System;
using System.Collections.Generic;
using Infrastructure.Common.Windows.ViewModels;

namespace VideoModule.ViewModels
{
	public class MultiLayoutCameraViewModel : BaseViewModel
	{
		public List<CameraViewModel> CameraViewModels { get; private set; }

		public MultiLayoutCameraViewModel(List<CameraViewModel> cameraViewModels)
		{
			CameraViewModels = cameraViewModels;
		}
	}
}