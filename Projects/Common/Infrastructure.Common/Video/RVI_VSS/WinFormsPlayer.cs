using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Entities.DeviceOriented;
using FiresecAPI.Models;
using GalaSoft.MvvmLight.Threading;

namespace Infrastructure.Common.Video.RVI_VSS
{
	public partial class WinFormsPlayer : UserControl
	{
		IStream ExtraStream { get; set; }
		PlayBackDeviceRecord Record { get; set; }
		bool IsStarted { get; set; }
		bool IsPaused { get; set; }
		public WinFormsPlayer()
		{
			InitializeComponent();
		}

		static WinFormsPlayer()
		{
			DispatcherHelper.Initialize();
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

		public PropertyChangedEventHandler PropertyChangedEvent;
		public List<IChannel> Connect(Camera camera)
		{
			try
			{
				var device = RviVssHelper.Devices.FirstOrDefault(x => x.IP == camera.Ip && x.Port == camera.Port && x.UserName == camera.Login && x.Password == camera.Password);
				if (device == null)
				{
					var deviceSi = new DeviceSearchInfo(camera.Ip, camera.Port, camera.Login, camera.Password);
					device = DeviceManager.Instance.GetDevice(deviceSi);
					device.PropertyChanged -= DeviceOnPropertyChanged;
					device.PropertyChanged += DeviceOnPropertyChanged;
					device.Authorize();
					RviVssHelper.Devices.Add(device);
				}
				return device.Channels.ToList();
			}
			catch(Exception e)
			{
				throw new Exception(e.Message);
			}
		}

		private void DeviceOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{
			PropertyChangedEvent(sender, propertyChangedEventArgs);
		}

		public bool Start(Camera camera, int channelNumber)
		{
			try
			{
				var device = RviVssHelper.Devices.FirstOrDefault(x => x.IP == camera.Ip && x.Port == camera.Port && x.UserName == camera.Login && x.Password == camera.Password);
				if (device == null)
					return false;
				Invalidate();
				var channel = device.Channels.First(channell => channell.ChannelNumber == channelNumber);
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
				record.StartPlaybackByTime(Handle, record.NetRecordFileInfo.StartTime);
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
				IsPaused = true;
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