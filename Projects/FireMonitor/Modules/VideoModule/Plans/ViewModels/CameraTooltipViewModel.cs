using System.Collections.ObjectModel;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using FiresecAPI.Models;

namespace VideoModule.Plans.ViewModels
{
	public class CameraTooltipViewModel : BaseViewModel
	{
		public XStateClass StateClass { get; private set; }
		public Camera Camera { get; private set; }

		public CameraTooltipViewModel(Camera camera)
		{
			Camera = camera;
			StateClass = camera.StateClass;
		}

		public void OnStateChanged()
		{
			OnPropertyChanged(() => StateClass);
		}
	}
}