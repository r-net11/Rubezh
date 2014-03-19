using System;
using System.Linq;
using System.Windows.Forms;
using Entities.DeviceOriented;
using FiresecAPI.Models;

namespace Infrastructure.Common.Video.RVI_VSS
{
	public partial class WinFormsPlayer : UserControl
	{
		public WinFormsPlayer()
		{
			InitializeComponent();
		}

		Stream ExtraStream { get; set; }
		Channel FirstChannel { get; set; }
		public void StopVideo()
		{
			if (ExtraStream != null)
				ExtraStream.RemovePlayHandle(Handle);
			Invalidate();
			ExtraStream = null;
		}

		public void StartVideo(Camera camera)
		{
			try
			{
				StopVideo();
				var perimeter = SystemPerimeter.Instance;
				var deviceSI = new DeviceSearchInfo(camera.Address, camera.Port);
				var device = perimeter.AddDevice(deviceSI);
				device.Authorize();
				FirstChannel = device.Channels.First(channell => channell.ChannelNumber == 0);
				ExtraStream = FirstChannel.Streams.First(stream => stream.StreamType == StreamTypes.ExtraStream1);
				ExtraStream.AddPlayHandle(Handle);
			}
			catch {}
		}

		private void ToolStripMenuItem1OnClick(object sender, EventArgs eventArgs)
		{
			var records = FirstChannel.QueryRecordFiles(new DateTime(2014, 02, 24, 19, 00, 00), new DateTime(2014, 02, 24, 20, 00, 00));
			var record = records.FirstOrDefault();
			StopVideo();
			record.StartPlaybackByTime(Handle, new DateTime(2014, 02, 24, 19, 26, 11));

			//record.StopPlayBack();
		}
	}
}
