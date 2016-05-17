using Microsoft.VisualStudio.TestTools.UnitTesting;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKManager2.Test
{
	public partial class ConfigurationTest
	{
		[TestMethod]
		public void AddDeviceToZoneTest()
		{
			var device = AddDevice(kauDevice11, GKDriverType.RSR2_SmokeDetector);
			var zone = new GKZone();
			GKManager.AddZone(zone);
			GKManager.AddDeviceToZone(device, zone);
			Assert.IsTrue(device.ZoneUIDs.Contains(zone.UID));
			Assert.IsTrue(device.Zones.Contains(zone));
			Assert.IsTrue(zone.Devices.Contains(device));
		}
	}
}
