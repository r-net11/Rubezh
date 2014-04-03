using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Entities.DeviceOriented;
using Device = Entities.DeviceOriented.Device;

namespace Infrastructure.Common.Video.RVI_VSS
{
	public partial class WinFormsPlayer : UserControl
	{
		Stream ExtraStream { get; set; }
		PlayBackDeviceRecord Record { get; set; }
		Device Device { get; set; }
		bool IsConnected { get; set; }
		bool IsStarted { get; set; }
		bool IsPaused { get; set; }
		public WinFormsPlayer()
		{
			InitializeComponent();
		}

		private int _speed;
		private int Speed
		{
			get { return _speed; }
			set
			{
				_speed = value;
				switch (_speed)
				{
					case 1: label.Text = "x1/8"; break;
					case 2: label.Text = "x1/4"; break;
					case 3: label.Text = "x1/2"; break;
					case 4: label.Text = "x1"; break;
					case 5: label.Text = "x2"; break;
					case 6: label.Text = "x4"; break;
					case 7: label.Text = "x8"; break;
					default: label.Text = ""; break;
				}
			}
		}

		public void Stop()
		{
			if (!IsStarted)
				return;
			if (ExtraStream != null)
			{
				ExtraStream.RemovePlayHandle(Handle);
				IsStarted = false;
				label.Text = "";
			}
			Invalidate();
			ExtraStream = null;
		}

		public List<Channel> Connect(string ipAddress, int port, string login, string password)
		{
			try
			{
				var device = SystemPerimeter.Instance.Devices.FirstOrDefault(x => x.IP == ipAddress && x.Port == port);
				if (device == null)
				{
					var deviceSi = new DeviceSearchInfo(ipAddress, port, login, password);
					device = SystemPerimeter.Instance.AddDevice(deviceSi);
				}
				Device = device;
				System.Diagnostics.Trace.WriteLine("Devices.Count=" + SystemPerimeter.Instance.Devices.Count().ToString());
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
				IsStarted = true;
				Speed = 0;
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
				IsStarted = true;
				Speed = 4;
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool Pause(PlayBackDeviceRecord record, bool pausePlayBack)
		{
			try
			{
				if (!IsStarted)
					return false;
				Record = record;
				record.PausePlayBack(pausePlayBack);
				IsPaused = pausePlayBack;
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool Stop(PlayBackDeviceRecord record)
		{
			try
			{
				if (!IsStarted)
					return false;
				Record = record;
				record.StopPlayBack();
				IsStarted = false;
				Speed = 0;
				Invalidate();
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool Fast(PlayBackDeviceRecord record)
		{
			try
			{
				if (!IsStarted)
					return false;
				if (Speed == 7)
					return false;
				Record = record;
				record.FastPlayBack();
				Speed++;
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool Slow(PlayBackDeviceRecord record)
		{
			try
			{
				if (!IsStarted)
					return false;
				if (Speed == 1)
					return false;
				Record = record;
				record.SlowPlayBack();
				Speed--;
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}