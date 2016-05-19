using GKProcessor;
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
			GKManager.AddGuardZone(guardZone);
#region forzone	
			// проверка добавления устройства в зону
			GKManager.AddDeviceToGuardZone(guardZone, guardZoneDevice);
			Assert.IsTrue(device.GuardZones.Contains(guardZone));
			Assert.IsTrue(guardZone.GuardZoneDevices.Any(x => x.DeviceUID == device.UID));
			Assert.IsTrue(device.InputDependentElements.Contains(guardZone));
			Assert.IsTrue(guardZone.OutputDependentElements.Contains(device));

			GKManager.RemoveDeviceFromGuardZone(device, guardZone);
			Assert.IsFalse(guardZone.GuardZoneDevices.Any(x => x.Device.UID == device.UID));
			Assert.IsFalse(device.GuardZones.Contains(guardZone));
			Assert.IsFalse(device.InputDependentElements.Contains(guardZone));
			Assert.IsFalse(guardZone.OutputDependentElements.Contains(device));
#endregion

#region fordevice
			// проверка добавления зоны в устройства
			GKManager.ChangeDeviceGuardZones(device, new List<GKDeviceGuardZone>() {new GKDeviceGuardZone{GuardZone = guardZone, GuardZoneUID = guardZone.UID}});
			Assert.IsTrue(device.GuardZones.Contains(guardZone));
			Assert.IsTrue(guardZone.GuardZoneDevices.Any(x => x.DeviceUID == device.UID));
			Assert.IsTrue(device.InputDependentElements.Contains(guardZone));
			Assert.IsTrue(guardZone.OutputDependentElements.Contains(device));

			var guardZone_2 = new GKGuardZone();
			GKManager.AddGuardZone(guardZone_2);
			GKManager.ChangeDeviceGuardZones(device, new List<GKDeviceGuardZone>() { new GKDeviceGuardZone { GuardZone = guardZone_2, GuardZoneUID = guardZone_2.UID }});
			Assert.IsTrue(device.GuardZones.Contains(guardZone_2));
			Assert.IsTrue(guardZone_2.GuardZoneDevices.Any(x => x.DeviceUID == device.UID));
			Assert.IsTrue(device.InputDependentElements.Contains(guardZone_2));
			Assert.IsTrue(guardZone_2.OutputDependentElements.Contains(device));

			Assert.IsFalse(device.GuardZones.Contains(guardZone));
			Assert.IsFalse(guardZone.GuardZoneDevices.Any(x => x.DeviceUID == device.UID));
			Assert.IsFalse(device.InputDependentElements.Contains(guardZone));
			Assert.IsFalse(guardZone.OutputDependentElements.Contains(device));
#endregion
			//удалениy зоны
			GKManager.AddDeviceToGuardZone(guardZone, guardZoneDevice);
			GKManager.RemoveGuardZone(guardZone);
			Assert.IsFalse(device.GuardZones.Any(x => x.UID == guardZone.UID));
			Assert.IsFalse(device.InputDependentElements.Any(x => x.UID == guardZone.UID));

			// удаление устройства
			GKManager.ChangeDeviceGuardZones(device, new List<GKDeviceGuardZone>() { new GKDeviceGuardZone { GuardZone = guardZone_2, GuardZoneUID = guardZone_2.UID } });
			GKManager.RemoveDevice(device);
			Assert.IsFalse(guardZone_2.GuardZoneDevices.Any(x => x.DeviceUID == device.UID));
			Assert.IsFalse(guardZone_2.OutputDependentElements.Any(x => x.UID == device.UID));

		}
		/// <summary>
		/// тест на добавления устройства в логику задержки,направления,устройства 
		/// и праверку на отстутсвия устройства в логики при его удалении
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
			Assert.IsFalse(device.OutputDependentElements.Any(x => x.UID == device.UID));
			Assert.IsFalse(device.InputDependentElements.Any(x => x.UID == device.UID));

			var delay = new GKDelay();
			GKManager.AddDelay(delay);
			GKManager.SetDelayLogic(delay, gkLogic);
			Assert.IsTrue(delay.Logic.OnClausesGroup.Clauses.Any(x => x.DeviceUIDs.Contains(device.UID)));
			Assert.IsTrue(device.OutputDependentElements.Any(x => x.UID == delay.UID));
			Assert.IsTrue(delay.InputDependentElements.Any(x => x.UID == device.UID));

			var direction = new GKDirection();
			GKManager.AddDirection(direction);
			GKManager.SetDirectionLogic(direction,gkLogic);
			Assert.IsTrue(direction.Logic.OnClausesGroup.Clauses.Any(x => x.DeviceUIDs.Any()));
			Assert.IsTrue(device.OutputDependentElements.Any(x => x.UID == direction.UID));
			Assert.IsTrue(direction.InputDependentElements.Any(x => x.UID == device.UID));

			GKManager.RemoveDevice(device);
			Assert.IsFalse(delay.Logic.OnClausesGroup.Clauses.Any(x => x.DeviceUIDs.Contains(device.UID)));
			Assert.IsFalse(direction.Logic.OnClausesGroup.Clauses.Any(x => x.DeviceUIDs.Contains(device.UID)));
			Assert.IsFalse(delay.InputDependentElements.Any(x => x.UID == device.UID));
			Assert.IsFalse(direction.InputDependentElements.Any(x => x.UID == device.UID));

		}

		/// <summary>
		/// тест на добавления устройства в логику и устройств входящих в  МПТ,НС,ТД  
		/// и праверку на отстутсвия устройства в логики и устройств входящих в  МПТ,НС,ТД  при его удалении
		/// </summary>
		[TestMethod]
		public void RemoveDeviceTestLogicForMptNsDoor()
		{
			var device = AddDevice(kauDevice11, GKDriverType.RSR2_AM_1);
			var device2 = AddDevice(kauDevice11, GKDriverType.RSR2_AM_1);
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
			Assert.IsTrue(mpt.InputDependentElements.Contains(device));
			Assert.IsTrue(device.OutputDependentElements.Contains(mpt));

			var pump = new GKPumpStation();
			GKManager.AddPumpStation(pump);
			GKManager.SetPumpStationStartLogic(pump, gkLogic);
			GKManager.ChangePumpDevices(pump, new List<GKDevice>() {device});
			Assert.IsTrue(pump.StartLogic.OnClausesGroup.Clauses.Any(x => x.DeviceUIDs.Contains(device.UID)));
			Assert.IsTrue(pump.NSDevices.Contains(device));
			Assert.IsTrue(pump.InputDependentElements.Contains(device));
			Assert.IsTrue(device.OutputDependentElements.Contains(pump));

			var door = new GKDoor();
			GKManager.AddDoor(door);
			GKManager.SetDoorOpenRegimeLogic(door, gkLogic);
			GKManager.SetDoorCloseRegimeLogic(door, gkLogic);
			GKManager.ChangeEnterButtonDevice(door, device);
			Assert.IsTrue(door.EnterButton == device);
			Assert.IsTrue(door.EnterButtonUID == device.UID);
			Assert.IsTrue(device.Door == door);
			Assert.IsTrue(device.OutputDependentElements.Contains(door));
			Assert.IsTrue(door.InputDependentElements.Contains(device));
			door.EnterButton = null;
			door.EnterButtonUID = Guid.Empty;

			GKManager.ChangeExitButtonDevice(door, device);
			Assert.IsTrue(door.ExitButton == device);
			Assert.IsTrue(door.ExitButtonUID == device.UID);
			Assert.IsTrue(device.Door == door);
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
			Assert.IsFalse(mpt.InputDependentElements.Any(x => x.UID ==  device.UID));
			Assert.IsFalse(pump.StartLogic.OnClausesGroup.Clauses.Any(x => x.DeviceUIDs.Contains(device.UID)));
			Assert.IsFalse(pump.InputDependentElements.Any(x => x.UID == device.UID));
			Assert.IsFalse(mpt.MPTDevices.Any(x => x.DeviceUID == device.UID));
			Assert.IsFalse(pump.NSDevices.Contains(device));
			Assert.IsFalse(door.InputDependentElements.Any(x => x.UID == device.UID));
		}
		[TestMethod]
		public void RemoveGroupDeviceTest()
		{
			var device = AddDevice(kauDevice11, GKDriverType.RSR2_OPSZ);
			GKManager.UpdateConfiguration();
			var zone = new GKZone();
			GKManager.AddZone(zone);
			GKManager.AddDeviceToZone(device.Children[1], zone);
			Assert.IsTrue(device.Children[1].Zones.Contains(zone));
			Assert.IsTrue(zone.Devices.Contains(device.Children[1]));
			GKManager.RemoveDevice(device);
			Assert.IsFalse(zone.Devices.Any());
		}

		//[TestMethod]
		//public void ChangeGroupDeviceWithZoneTest()
		//{
		//	var device = AddDevice(kauDevice11, GKDriverType.RSR2_OPSZ);
		//	GKManager.UpdateConfiguration();
		//	var zone = new GKZone();
		//	GKManager.AddZone(zone);
		//	GKManager.AddDeviceToZone(device.Children[1], zone);
		//	GKManager.ChangeDriver(device, RSR2_AM_1_Helper.Create());
		//	Assert.IsTrue(device.DriverType == GKDriverType.RSR2_AM_1);
		//	Assert.IsTrue(device.Children.Count ==0);
		//	Assert.IsFalse(device.Driver.IsGroupDevice);
		//	Assert.IsTrue(device.Driver.GroupDeviceChildrenCount == 0);
		//	Assert.IsTrue(device.InputDependentElements.Count == 0);
		//	Assert.IsTrue(zone.OutputDependentElements.Count == 0);

		//}
		//[TestMethod]
		//public void ChangeGroupDeviceWithLogicTest()
		//{
		//	var device = AddDevice(kauDevice11, GKDriverType.RSR2_MDU);
		//	var device_2 = AddDevice(kauDevice11, GKDriverType.RSR2_MDU);
		//	GKManager.UpdateConfiguration();

		//	var clause = new GKClause
		//	{
		//		ClauseOperationType = ClauseOperationType.AllDevices,
		//		DeviceUIDs = { device.UID }
		//	};

		//	var gkLogic = new GKLogic();
		//	gkLogic.OnClausesGroup.Clauses.Add(clause);
		//	var delay = new GKDelay();
		//	GKManager.AddDelay(delay);
		//	GKManager.SetDelayLogic(delay, gkLogic);
		//	GKManager.ChangeDriver(device, RSR2_AM_4_Group_Helper.Create());
		//	Assert.IsTrue(device.DriverType == GKDriverType.RSR2_AM_4);
		//	Assert.IsTrue(device.IntAddress ==1);
		//	Assert.IsTrue(device_2.IntAddress == 5);
		//	Assert.IsTrue(device.Children.Count == 4);
		//	Assert.IsTrue(device.Driver.IsGroupDevice);
		//	Assert.IsTrue(device.Driver.GroupDeviceChildrenCount == 4);
		//	Assert.IsTrue(device.InputDependentElements.Count == 0);
		//	Assert.IsTrue(delay.OutputDependentElements.Count == 0);
		//}
	}
}