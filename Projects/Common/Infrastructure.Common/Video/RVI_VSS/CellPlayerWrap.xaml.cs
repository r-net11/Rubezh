using System.Linq;
using Entities.DeviceOriented;
using FiresecAPI.Models;
using RVI_VSS.ViewModels;

namespace Infrastructure.Common.Video.RVI_VSS
{
	public partial class CellPlayerWrap
	{
		public CellPlayerWrap()
		{
			InitializeComponent();
		}

		public Camera Camera { get; private set; }
		public void InitializeCamera(Camera camera)
		{
			Camera = camera;
			var perimeter = SystemPerimeter.Instance;
			var deviceSearchInfo = new DeviceSearchInfo(camera.Address, 37777);
			try
			{
				var device = perimeter.AddDevice(deviceSearchInfo);
				device.Authorize();
				var firstChannel = device.Channels.First(channel => channel.ChannelNumber == 0);
				var videoCell = new VideoCell
				{
					Channel = new ChannelViewModel(firstChannel)
				};
				FormsPlayer.VideoCell = videoCell;
			}
			catch {}
		}
	}
}