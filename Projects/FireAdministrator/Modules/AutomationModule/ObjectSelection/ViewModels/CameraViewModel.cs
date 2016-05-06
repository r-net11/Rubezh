using StrazhAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class CameraViewModel : BaseViewModel
	{
		public Camera Camera { get; private set; }

		public CameraViewModel(Camera camera)
		{
			Camera = camera;
		}
	}
}