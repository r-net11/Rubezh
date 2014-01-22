using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Common;
using FiresecClient;
using XFiresecAPI;

namespace GKProcessor
{
	internal static class TimeSynchronisationHelper
	{
		static Thread Thread;
		static AutoResetEvent AutoResetEvent = new AutoResetEvent(false);

		public static void Start()
		{
			if (Thread == null)
			{
				Thread = new Thread(OnRun);
				Thread.Name = "GK TimeSynchronisationHelper";
				Thread.Start();
			}
		}

		public static void Stop()
		{
			if (AutoResetEvent != null)
			{
				AutoResetEvent.Set();
				if (Thread != null)
				{
					Thread.Join(TimeSpan.FromSeconds(1));
				}
			}
		}

		static void OnRun()
		{
			while (true)
			{
				try
				{
					foreach (var device in XManager.DeviceConfiguration.RootDevice.Children)
					{
						if (device.DriverType == XDriverType.GK)
						{
							WriteDateTime(device);
						}
					}

					AutoResetEvent = new AutoResetEvent(false);
					if (AutoResetEvent.WaitOne(TimeSpan.FromDays(1)))
					{
						break;
					}
				}
				catch (Exception e)
				{
					Logger.Error(e, "TimeSynchronisationHelper.OnRun");
				}
			}
		}

		static bool WriteDateTime(XDevice device)
		{
			var dateTime = DateTime.Now;
			var bytes = new List<byte>();
			bytes.Add((byte)dateTime.Day);
			bytes.Add((byte)dateTime.Month);
			bytes.Add((byte)(dateTime.Year - 2000));
			bytes.Add((byte)dateTime.Hour);
			bytes.Add((byte)dateTime.Minute);
			bytes.Add((byte)dateTime.Second);
			var sendResult = SendManager.Send(device, 6, 5, 0, bytes);
			return !sendResult.HasError;
		}
	}
}