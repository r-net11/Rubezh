using FiresecAPI;
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

		public string PresentationAddress
		{
			get
			{
				if (Camera.CameraType != CameraType.Channel)
					return Camera.Ip;
				return (Camera.ChannelNumber + 1).ToString();
			}
		}

		public string PresentationName
		{
			get
			{
				if (Camera.CameraType == CameraType.Channel)
					return Camera.Name + " " + (Camera.ChannelNumber + 1) + " (" + Camera.Ip + ")";
				return Camera.PresentationName;
			}
		}

		public bool IsDvr
		{
			get
			{
				return Camera.CameraType == CameraType.Dvr;
			}
		}
	}
}