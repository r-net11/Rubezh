using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Common;
using FiresecAPI.SKD;

namespace FiresecService
{
	public static class SKDDaylyUpdateProcessor
	{
		static Thread Thread;
		static AutoResetEvent AutoResetEvent = new AutoResetEvent(false);

		static SKDDaylyUpdateProcessor()
		{
		}

		public static void Update()
		{
			foreach (var device in SKDManager.Devices)
			{
				if (device.Driver.IsController)
				{
					ChinaSKDDriver.Processor.SyncronyseTime(device.UID);
				}
			}
		}

		public static void Start()
		{
			if (Thread == null)
			{
				Thread = new Thread(OnRun);
				Thread.Name = "SKDDaylyUpdateProcessor";
				Thread.IsBackground = true;
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
			AutoResetEvent = new AutoResetEvent(false);
			while (true)
			{
				try
				{
					var hoursLeft = 25 - DateTime.Now.TimeOfDay.Hours;
					if (AutoResetEvent.WaitOne(TimeSpan.FromHours(hoursLeft)))
					{
						break;
					}

					Update();
				}
				catch (Exception e)
				{
					Logger.Error(e, "SKDDaylyUpdateProcessor.OnRun");
				}
			}
		}
	}
}
