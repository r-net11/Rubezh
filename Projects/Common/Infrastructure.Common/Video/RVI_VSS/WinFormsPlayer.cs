using System;
using System.Linq;
using System.Windows.Forms;
using Entities.DeviceOriented;
using FiresecAPI.Models;
using Device = Entities.DeviceOriented.Device;

namespace Infrastructure.Common.Video.RVI_VSS
{
	public partial class WinFormsPlayer : UserControl
	{
		Stream ExtraStream { get; set; }
		Channel FirstChannel { get; set; }
		PlayBackDeviceRecord Record { get; set; }

		public WinFormsPlayer()
		{
			InitializeComponent();
		}

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

		public void StartVideo(Device device)
		{
			try
			{
				StopVideo();
				FirstChannel = device.Channels.First(channell => channell.ChannelNumber == 0);
				ExtraStream = FirstChannel.Streams.First(stream => stream.StreamType == StreamTypes.ExtraStream1);
				ExtraStream.AddPlayHandle(Handle);
			}
			catch { }
		}

		public void StartVideo(PlayBackDeviceRecord record)
		{
			try
			{
				if (Record != null)
					Record.StopPlayBack();
				Record = record;
				record.StartPlaybackByFile(Handle);
			}
			catch { }
		}
	}
}
