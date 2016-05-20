using Infrastructure.Common.TreeList;
using RubezhAPI.Models;

namespace VideoModule.ViewModels
{
	public class SimpleCameraViewModel : TreeNodeViewModel<SimpleCameraViewModel>
	{
		public Camera Camera { get; private set; }
		public string PresentationName { get; private set; }
		public string PresentationAddress { get; private set; }
		public string ImageSource { get; private set; }
		public bool IsCamera { get { return Camera != null; } }
		public SimpleCameraViewModel(Camera camera)
		{
			Camera = camera;
			PresentationName = camera.Name;
			ImageSource = camera.ImageSource;
		}
		public SimpleCameraViewModel(RviServer server)
		{
			PresentationName = server.PresentationName;
			ImageSource = server.ImageSource;
		}
		public SimpleCameraViewModel(RviDevice device)
		{
			PresentationName = device.Name;
			PresentationAddress = device.Ip;
			ImageSource = device.ImageSource;
		}
		public bool IsOnPlan
		{
			get { return Camera != null && Camera.PlanElementUIDs.Count > 0; }
		}
	}
}