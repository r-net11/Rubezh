using Infrastructure.Common.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKProcessor.Test
{
	public partial class ValidationTest
	{
		/// <summary>
		/// Валидация должна проверять, что В МПТ настроена логика включения
		/// </summary>
		[TestMethod]
		public void ValidateEmptyMPTDevice()
		{
			var mpt = new GKMPT();
			GKManager.AddMPT(mpt);
			var errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "Отсутствует логика включения"));
		}

		/// <summary>
		/// Валидация должна проверять, что к МПТ подключены устройства
		/// </summary>
		[TestMethod]
		public void ValidateEmptyMPTLogic()
		{
			var mpt = new GKMPT();
			GKManager.AddMPT(mpt);
			var errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К МПТ не подключены устройства"));
		}

		/// <summary>
		/// Валидация должна проверять, что устройства, подключенные к МПТ, не совпадают
		/// </summary>
		[TestMethod]
		public void ValidateMPTSameDevices()
		{
			var device1 = AddDevice(kauDevice11, GKDriverType.RSR2_AM_1);
			var mpt = new GKMPT();
			GKManager.AddMPT(mpt);
			mpt.MPTDevices.Add(new GKMPTDevice { MPTDeviceType = GKMPTDeviceType.HandStart, DeviceUID = device1.UID });
			mpt.MPTDevices.Add(new GKMPTDevice { MPTDeviceType = GKMPTDeviceType.HandStop, DeviceUID = device1.UID });
			var errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "Дублируются устройства, входящие в МПТ"));
		}
		/// <summary>
		/// Устройства МПТ должны быть определенного типа. Например, такое может произойти при смене типа
		/// </summary>
		[TestMethod]
		public void ValidateWrongMPTDevice()
		{
			var device = AddDevice(kauDevice11, GKDriverType.RSR2_AM_1);
			var mpt = new GKMPT();
			GKManager.AddMPT(mpt);
			mpt.MPTDevices.Add(new GKMPTDevice { MPTDeviceType = GKMPTDeviceType.Bomb, DeviceUID = device.UID });
			var errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "МПТ содержит устройство неверного типа"));
		}
	}
}