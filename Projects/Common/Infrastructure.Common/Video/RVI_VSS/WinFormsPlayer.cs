using System;
using System.Activities.Expressions;
using System.Drawing;
using System.Linq;
using System.Threading;
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

				var firstChannel = device.Channels.First(channell => channell.ChannelNumber == 0);

				//var form = new Form();

				ExtraStream = firstChannel.Streams.First(stream => stream.StreamType == StreamTypes.ExtraStream1);

				//var records = firstChannel.QueryRecordFiles(new DateTime(2014, 02, 24, 19, 00, 00), new DateTime(2014, 02, 24, 20, 00, 00));

				ExtraStream.AddPlayHandle(Handle);

				//var record = records.FirstOrDefault();

				//form.MouseClick += (sender, args) =>
				//{
				//    if (args.Button == MouseButtons.Left)
				//    {
				//        record.PausePlayBack(true);
				//    }

				//    if (args.Button == MouseButtons.Right)
				//    {
				//        record.PausePlayBack(false);
				//    }

				//    if (args.Button == MouseButtons.Middle)
				//    {
				//        record.StepPlayBack(true);
				//    }
				//};

				//record.StartPlaybackByTime(form.Handle, new DateTime(2014, 02, 24, 19, 15, 00));

				//form.ShowDialog();

				//record.StopPlayBack();

			}
			catch {}


			//Camera = camera;
			//var perimeter = SystemPerimeter.Instance;
			//var deviceSearchInfo = new DeviceSearchInfo(camera.Address, 37777);
			//try
			//{
			//    var device = perimeter.AddDevice(deviceSearchInfo);
			//    device.Authorize();
			//    var firstChannel = device.Channels.First(channel => channel.ChannelNumber == 0);
			//    var videoCell = new VideoCell
			//    {
			//        Channel = new ChannelViewModel(firstChannel)
			//    };
			//    FormsPlayer.VideoCell = videoCell;
			//}
			//catch {}
		}
	}
}
