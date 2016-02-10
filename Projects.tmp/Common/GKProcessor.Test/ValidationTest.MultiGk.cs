using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using GKModule.Validation;
using Infrastructure.Common.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RubezhAPI;

namespace GKProcessor.Test
{
	public partial class ValidationTest
	{
		[TestMethod]
		public void TestFireZoneInMultiGk()
		{
			var device1 = AddDevice(kauDevice11, GKDriverType.RSR2_HandDetector);
			var device2 = AddDevice(kauDevice21, GKDriverType.RSR2_HandDetector);
			var zone = new GKZone();
			GKManager.Zones.Add(zone);
			device1.ZoneUIDs.Add(zone.UID);
			device2.ZoneUIDs.Add(zone.UID);
			var validator = new Validator();
			var errors = validator.Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "Содержится в нескольких ГК"));
		}

		[TestMethod]
		public void TestGuardZoneInMultiGk()
		{
			var device1 = AddDevice(kauDevice11, GKDriverType.RSR2_GuardDetector);
			var device2 = AddDevice(kauDevice21, GKDriverType.RSR2_GuardDetector);
			var guardZoneDevice1 = new GKGuardZoneDevice {Device = device1, DeviceUID = device1.UID};
			var guardZoneDevice2 = new GKGuardZoneDevice {Device = device2, DeviceUID = device2.UID};
			var guardZone = new GKGuardZone();
			GKManager.GuardZones.Add(guardZone);
			guardZone.GuardZoneDevices.Add(guardZoneDevice1);
			guardZone.GuardZoneDevices.Add(guardZoneDevice2);
			GKManager.AddDeviceToGuardZone(guardZone, guardZoneDevice1);
			GKManager.AddDeviceToGuardZone(guardZone, guardZoneDevice2);
			var validator = new Validator();
			var errors = validator.Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "Содержится в нескольких ГК"));
		}

		[TestMethod]
		public void TestDirectionInMultiGk()
		{
			var device1 = AddDevice(kauDevice11, GKDriverType.RSR2_GuardDetector);
			var device2 = AddDevice(kauDevice21, GKDriverType.RSR2_GuardDetector);
			var direction = new GKDirection();
			GKManager.Directions.Add(direction);
			var clause = new GKClause
			{
				ClauseOperationType = ClauseOperationType.AllDevices,
				StateType = GKStateBit.Failure,
				DeviceUIDs = {device1.UID, device2.UID}
			};
			direction.Logic.OnClausesGroup.Clauses.Add(clause);
			var validator = new Validator();
			var errors = validator.Validate();
			Assert.IsTrue(
				errors.Any(
					x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "Содержится в нескольких ГК"));
		}

		[TestMethod]
		public void TestDelayInMultiGk()
		{
			var device1 = AddDevice(kauDevice11, GKDriverType.RSR2_GuardDetector);
			var device2 = AddDevice(kauDevice21, GKDriverType.RSR2_GuardDetector);
			var delay = new GKDelay();
			GKManager.Delays.Add(delay);
			var clause = new GKClause
			{
				ClauseOperationType = ClauseOperationType.AllDevices,
				StateType = GKStateBit.Failure,
				DeviceUIDs = {device1.UID, device2.UID}
			};
			delay.Logic.OnClausesGroup.Clauses.Add(clause);
			var validator = new Validator();
			var errors = validator.Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "Содержится в нескольких ГК"));
		}

		[TestMethod]
		public void TestDeviceInMultiGk()
		{
			var device1 = AddDevice(kauDevice11, GKDriverType.RSR2_GuardDetector);
			var device2 = AddDevice(kauDevice21, GKDriverType.RSR2_GuardDetector);
			var device = AddDevice(kauDevice22, GKDriverType.RSR2_RM_1);
			var clause = new GKClause
			{
				ClauseOperationType = ClauseOperationType.AllDevices,
				StateType = GKStateBit.Failure,
				DeviceUIDs = {device1.UID, device2.UID}
			};
			device.Logic.OnClausesGroup.Clauses.Add(clause);
			var validator = new Validator();
			var errors = validator.Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "Содержится в нескольких ГК"));
		}

		[TestMethod]
		public void TestMPTInMultiGk()
		{
			var device1 = AddDevice(kauDevice11, GKDriverType.RSR2_GuardDetector);
			var device2 = AddDevice(kauDevice21, GKDriverType.RSR2_GuardDetector);
			var mpt = new GKMPT();
			GKManager.MPTs.Add(mpt);
			var clause = new GKClause
			{
				ClauseOperationType = ClauseOperationType.AllDevices,
				StateType = GKStateBit.Failure,
				DeviceUIDs = {device1.UID, device2.UID}
			};
			mpt.MptLogic.OnClausesGroup.Clauses.Add(clause);
			var validator = new Validator();
			var errors = validator.Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "Содержится в нескольких ГК"));

			mpt.MptLogic = new GKLogic();
			clause = new GKClause
			{
				ClauseOperationType = ClauseOperationType.AllDevices,
				StateType = GKStateBit.Failure,
				DeviceUIDs = { device1.UID }
			};
			mpt.MptLogic.OnClausesGroup.Clauses.Add(clause);

			foreach (var deviceType in  new List<GKMPTDeviceType>(Enum.GetValues(typeof(GKMPTDeviceType)).Cast<GKMPTDeviceType>()))
			{
				mpt.MPTDevices = new List<GKMPTDevice>();
				var mptDevice = new GKMPTDevice { Device = device2, DeviceUID = device2.UID, MPTDeviceType = deviceType };
				mpt.MPTDevices.Add(mptDevice);
				validator = new Validator();
				errors = validator.Validate();
				Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "Содержится в нескольких ГК"));
			}

		}
	}
}
