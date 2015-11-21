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

		/// <summary>
		/// тест для проверки добавления зоны к устройтву и устройства к зоне
		/// и проверка коллекций после удаления устройства 
		/// </summary>
		[TestMethod]
		public void RemoveDeviceTestGuardZone()
		{
			var device = AddDevice(kauDevice11, GKDriverType.RSR2_GuardDetector);
			var guardZone = new GKGuardZone();
			var guardZoneDevice = new GKGuardZoneDevice {Device = device,DeviceUID = device.UID };
			guardZone.GuardZoneDevices.Add(guardZoneDevice);
			GKManager.AddGuardZone(guardZone);
			GKManager.AddDeviceToGuardZone(device, guardZone,guardZone.GuardZoneDevices[0]);
			Assert.IsTrue(device.GuardZones.Contains(guardZone));
			Assert.IsTrue(guardZone.GuardZoneDevices.Any(x => x.DeviceUID == device.UID));
			GKManager.RemoveDeviceFromGuardZone(device, guardZone);
			Assert.IsFalse(guardZone.GuardZoneDevices.Any(x => x.Device.UID == device.UID));
		}
		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void RemoveDeviceTestLogicForDelayAndDirection()
		{
			var device = AddDevice(kauDevice11, GKDriverType.RSR2_MDU);
			GKManager.UpdateConfiguration();

			var clause = new GKClause
			{
				ClauseOperationType = ClauseOperationType.AllDevices,
				DeviceUIDs = { device.UID }
			};

			var gkLogic = new GKLogic();
			gkLogic.OnClausesGroup.Clauses.Add(clause);

			GKManager.SetDeviceLogic(device, gkLogic);
			Assert.IsTrue(device.Logic.OnClausesGroup.Clauses.Any(x => x.DeviceUIDs.Contains(device.UID)));

			var delay = new GKDelay();
			GKManager.AddDelay(delay);
			GKManager.SetDelayLogic(delay, gkLogic);
			Assert.IsTrue(delay.Logic.OnClausesGroup.Clauses.Any(x => x.DeviceUIDs.Contains(device.UID)));

			var direction = new GKDirection();
			GKManager.AddDirection(direction);
			GKManager.SetDirectionLogic(direction,gkLogic);
			Assert.IsTrue(direction.Logic.OnClausesGroup.Clauses.Any(x => x.DeviceUIDs.Any()));

			GKManager.RemoveDevice(device);
			Assert.IsFalse(delay.Logic.OnClausesGroup.Clauses.Any(x => x.DeviceUIDs.Contains(device.UID)));
			Assert.IsFalse(direction.Logic.OnClausesGroup.Clauses.Any(x => x.DeviceUIDs.Contains(device.UID)));

		}
		[TestMethod]
		public void RemoveDeviceTestLogicForMptNsDoor()
		{
			var device = AddDevice(kauDevice11, GKDriverType.RSR2_AM_1);
			GKManager.UpdateConfiguration();

			var clause = new GKClause
			{
				ClauseOperationType = ClauseOperationType.AllDevices,
				DeviceUIDs = { device.UID }
			};

			var gkLogic = new GKLogic();
			gkLogic.OnClausesGroup.Clauses.Add(clause);

			var mpt = new GKMPT();
			var gkMptDevice = new GKMPTDevice { Device = device, DeviceUID = device.UID };
			GKManager.AddMPT(mpt);
			GKManager.SetMPTLogic(mpt, gkLogic);
			mpt.MPTDevices.Add(gkMptDevice);
			Assert.IsTrue(mpt.MptLogic.OnClausesGroup.Clauses.Any(x => x.DeviceUIDs.Contains(device.UID)));
			Assert.IsTrue(mpt.MPTDevices.Any(x => x.DeviceUID == device.UID));

			var pump = new GKPumpStation();
			GKManager.AddPumpStation(pump);
			GKManager.SetPumpStationStartLogic(pump, gkLogic);
			GKManager.ChangePumpDevices(pump, new List<GKDevice>() {device});
			Assert.IsTrue(pump.StartLogic.OnClausesGroup.Clauses.Any(x => x.DeviceUIDs.Contains(device.UID)));
			Assert.IsTrue(pump.NSDevices.Contains(device));

			var door = new GKDoor();
			GKManager.AddDoor(door);
			GKManager.SetDoorOpenRegimeLogic(door, gkLogic);
			GKManager.SetDoorCloseRegimeLogic(door, gkLogic);
			GKManager.ChangeEnterButtonDevice(door, device);
			Assert.IsTrue(door.EnterButton == device);
			Assert.IsTrue(door.EnterButtonUID == device.UID);
			door.EnterButton = null;
			door.EnterButtonUID = Guid.Empty;

			GKManager.ChangeExitButtonDevice(door, device);
			Assert.IsTrue(door.ExitButton == device);
			Assert.IsTrue(door.ExitButtonUID == device.UID);
			door.ExitButton = null;
			door.ExitButtonUID = Guid.Empty;

			GKManager.ChangeLockControlDevice(door, device);
			Assert.IsTrue(door.LockControlDevice == device);
			Assert.IsTrue(door.LockControlDeviceUID == device.UID);
			Assert.IsTrue(device.Door == door);
			door.LockDevice = null;
			door.LockControlDeviceUID = Guid.Empty;

			GKManager.ChangeLockControlDeviceExit(door, device);
			Assert.IsTrue(door.LockControlDeviceExitUID == device.UID);
			Assert.IsTrue(door.LockControlDeviceExit == device);
			Assert.IsTrue(device.Door == door);
			door.LockDeviceExit = null;
			door.LockControlDeviceExitUID = Guid.Empty;

			GKManager.ChangeLockDevice(door, device);
			Assert.IsTrue(door.LockDevice == device);
			Assert.IsTrue(door.LockDeviceUID == device.UID);
			Assert.IsTrue(device.Door == door);
			door.LockDeviceUID = Guid.Empty;
			door.LockDevice = null;

			GKManager.ChangeLockDeviceExit(door, device);
			Assert.IsTrue(door.LockDeviceExit == device);
			Assert.IsTrue(door.LockDeviceExitUID == device.UID);
			Assert.IsTrue(device.Door == door);
			Assert.IsTrue(door.OpenRegimeLogic.OnClausesGroup.Clauses.Any(x => x.DeviceUIDs.Contains( device.UID)));
			Assert.IsTrue(door.CloseRegimeLogic.OnClausesGroup.Clauses.Any(x => x.DeviceUIDs.Contains(device.UID)));
			
			GKManager.RemoveDevice(device);
			Assert.IsFalse(mpt.MptLogic.OnClausesGroup.Clauses.Any(x => x.DeviceUIDs.Contains(device.UID)));
			Assert.IsFalse(pump.StartLogic.OnClausesGroup.Clauses.Any(x => x.DeviceUIDs.Contains(device.UID)));
			Assert.IsFalse(mpt.MPTDevices.Any(x => x.DeviceUID == device.UID));
			Assert.IsFalse(pump.NSDevices.Contains(device));
		}
	}
}