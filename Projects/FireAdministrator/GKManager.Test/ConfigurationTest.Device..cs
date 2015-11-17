using Microsoft.VisualStudio.TestTools.UnitTesting;
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
		public void RemoveDeviceTestZone()
		{
			var device = AddDevice(kauDevice11, GKDriverType.RSR2_SmokeDetector);
			var zone = new GKZone();
			GKManager.AddZone(zone);
			GKManager.AddDeviceToZone(device, zone);
			Assert.IsTrue(device.ZoneUIDs.Contains(zone.UID));
			Assert.IsTrue(device.Zones.Contains(zone));
			Assert.IsTrue(zone.Devices.Contains(device));
			GKManager.RemoveDevice(device);
			Assert.IsFalse(zone.Devices.Contains(device));
			Assert.IsFalse(GKManager.Devices.Any(x=> x.UID == device.UID));
		}


		[TestMethod]
		public void RemoveDeviceTestGuardZone()
		{
			var device = AddDevice(kauDevice11, GKDriverType.RSR2_GuardDetector);
			var guardZone = new GKGuardZone();
			GKManager.AddGuardZone(guardZone);
			GKManager.AddDeviceToGuardZone(device, guardZone);
			Assert.IsTrue(device.GuardZoneUIDs.Contains(guardZone.UID));
			Assert.IsTrue(device.GuardZones.Contains(guardZone));
			Assert.IsTrue(guardZone.GuardZoneDevices.Any(x => x.DeviceUID == device.UID));
			GKManager.RemoveDeviceFromGuardZone(device, guardZone);
			Assert.IsFalse(guardZone.GuardZoneDevices.Any(x => x.Device.UID == device.UID));
		}

		[TestMethod]
		public void RemoveDeviceTestLogic()
		{
			var device = AddDevice(kauDevice11, GKDriverType.RSR2_AM_1);
			var clause = new GKClause
			{
				ClauseOperationType = ClauseOperationType.AllDevices,
				DeviceUIDs = { device.UID }
			};

			var gkLogic = new GKLogic();
			gkLogic.OnClausesGroup.Clauses.Add(clause);

			//var logigDevice = AddDevice(kauDevice11, GKDriverType.RSR2_RM_1);
			//GKManager

			var delay = new GKDelay();
			GKManager.SetDelayLogic(delay, gkLogic);

			Assert.IsTrue(delay.Logic.OnClausesGroup.Clauses.Any(x=> x.DeviceUIDs.Contains(device.UID)));

			var direction = new GKDirection();
			GKManager.SetDirectionLogic(direction,gkLogic);

			

		}
	}
}