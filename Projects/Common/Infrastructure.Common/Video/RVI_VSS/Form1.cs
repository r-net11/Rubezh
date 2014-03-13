using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Entities.DeviceOriented;

namespace Infrastructure.Common.Video.RVI_VSS
{
	public partial class Form1 : UserControl
	{
		public Form1()
		{
			InitializeComponent();
			PlayVideo();
		}
		public IntPtr Handle { get; set; }
		public Entities.DeviceOriented.Stream MainStream { get; set; }

		public void PlayVideo()
		{
			var hwnd = VideoArea.Handle;
			var perimeter = SystemPerimeter.Instance;

			var deviceSearchInfo = new DeviceSearchInfo("172.16.2.36", 37777);

			var device = perimeter.AddDevice(deviceSearchInfo);

			device.Authorize();

			var firstChannel = device.Channels.First(channel => channel.ChannelNumber == 0);

			//var form = new Form();

			MainStream = firstChannel.Streams.First(stream => stream.StreamType == StreamTypes.MainStream);

			MainStream.AddPlayHandle(hWnd: hwnd);
		}

		private void VideoArea_MouseClick(object sender, MouseEventArgs e)
		{
			//PlayVideo();
		}
	}
}