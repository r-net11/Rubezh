using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class CameraViewModel : BaseViewModel
	{
		public Camera Camera { get; set; }

		public CameraViewModel(Camera camera)
		{
			Camera = camera;
		}

		public string PresentationAddress
		{
			get
			{
				return Camera.Ip;
			}
		}

		public string PresentationName
		{
			get
			{
				return Camera.PresentationName;
			}
		}
	}
}