using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI.GK;

namespace FiresecClient
{
	public partial class GKManager
	{
		public static GKDeviceConfiguration DeviceConfiguration { get; set; }

		static GKManager()
		{
			DeviceConfiguration = new GKDeviceConfiguration();
		}

		public static List<GKDevice> Devices
		{
			get { return DeviceConfiguration.Devices; }
		}
	}
}