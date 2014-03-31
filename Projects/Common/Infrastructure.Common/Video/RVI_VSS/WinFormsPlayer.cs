using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Forms;
using Entities.DeviceOriented;
using FiresecAPI.Models;
using Device = Entities.DeviceOriented.Device;

namespace Infrastructure.Common.Video.RVI_VSS
{
	public partial class WinFormsPlayer : UserControl
	{
		Stream ExtraStream { get; set; }
		PlayBackDeviceRecord Record { get; set; }
		Device Device { get; set; }

		public WinFormsPlayer()
		{
			InitializeComponent();
		}

		public void Stop()
		{
			if (ExtraStream != null)
			{
				ExtraStream.RemovePlayHandle(Handle);
			}
			//Invalidate();
			ExtraStream = null;
		}

		public List<Channel> Connect(string ipAddress, int port)
		{
			try
			{
				var deviceSi = new DeviceSearchInfo(ipAddress, port);
				Device = SystemPerimeter.Instance.AddDevice(deviceSi);
				return Device.Channels.ToList();
			}
			catch
			{
				return null;
			}
		}

		public bool Start(int channelNumber)
		{
			try
			{
				var channel = Device.Channels.First(channell => channell.ChannelNumber == channelNumber);
				ExtraStream = channel.Streams.First(stream => stream.StreamType == StreamTypes.ExtraStream1);
				ExtraStream.AddPlayHandle(Handle);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool Start(PlayBackDeviceRecord record)
		{
			try
			{
				if (Record != null)
					Record.StopPlayBack();
				Record = record;
				record.StartPlaybackByFile(Handle);
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
