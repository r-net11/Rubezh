using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace VideoModule.ViewModels
{
	public class CameraTooltipViewModel : BaseViewModel
	{
		public XStateClass StateClass { get; private set; }
		public Camera Camera { get; private set; }

		public CameraTooltipViewModel(Camera camera)
		{
			Camera = camera;
			StateClass = camera.CameraStateStateClass;
		}

		public void OnStateChanged()
		{
			OnPropertyChanged(() => StateClass);
		}
	}
}