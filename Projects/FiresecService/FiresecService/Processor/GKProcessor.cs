﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;
using FiresecService.Processor;
using FiresecClient;
using GKProcessor;

namespace FiresecService
{
	public static class GKProcessor
	{
		public static void Create()
		{
			XManager.DeviceConfiguration = ZipConfigurationHelper.GetDeviceConfiguration(); ;
			XManager.UpdateConfiguration();
			XManager.CreateStates();
			DescriptorsManager.Create();
			foreach (var gkDatabase in DescriptorsManager.GkDatabases)
			{
				foreach (var delay in gkDatabase.Delays)
				{
					delay.DelayState = new XDelayState();
				}
			}
		}

		public static void Start()
		{
			WatcherManager.Start();
		}

		public static void Stop()
		{
			WatcherManager.Stop();
		}
	}
}