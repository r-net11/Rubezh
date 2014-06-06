using FiresecAPI.Models;
using Infrastructure.Common.TreeList;

namespace AutomationModule.ViewModels
{
	public class CameraViewModel : TreeNodeViewModel<CameraViewModel>
	{
		public Camera Camera { get; set; }

		public CameraViewModel(Camera camera)
		{
			Camera = camera;
		}
	}
}