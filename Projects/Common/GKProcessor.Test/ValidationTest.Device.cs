using System.Linq;
using Infrastructure.Common.Windows.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RubezhAPI;
using RubezhAPI.GK;

namespace GKProcessor.Test
{
	public partial class ValidationTest
	{
		[TestMethod]
		//RG-1109 (Валидация настройки контроллера Wiegand при одновременном участии в ТД и в ОЗ)
		public void TestCardReader()
		{
			var rK = AddDevice(kauDevice11, GKDriverType.RK_RM);
			var cardReader = AddDevice(kauDevice11, GKDriverType.RSR2_CardReader);
			var codeReader = AddDevice(kauDevice11, GKDriverType.RSR2_CodeReader);
			var lockDevice = new GKGuardZoneDevice { DeviceUID = codeReader.UID, Device = codeReader };
			var enterDevice = new GKGuardZoneDevice { DeviceUID = cardReader.UID, Device = cardReader };
			var zone = new GKGuardZone { No = 1, Name = "Охранная зона" };
			var door = new GKDoor { No = 1, Name = "Точка доступа", DoorType = GKDoorType.TwoWay, EnterDevice = cardReader, EnterDeviceUID = cardReader.UID, ExitDevice = codeReader, ExitDeviceUID = codeReader.UID, LockDevice = rK, LockDeviceUID = codeReader.UID };
			GKManager.AddGuardZone(zone);
			GKManager.AddDeviceToGuardZone(zone, lockDevice);
			GKManager.AddDeviceToGuardZone(zone, enterDevice);
			GKManager.AddDoor(door);
			enterDevice.CodeReaderSettings.SetGuardSettings = new GKCodeReaderSettingsPart { CodeReaderEnterType = GKCodeReaderEnterType.CodeOnly };
			var errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "Котроллер Wiegand используется в точке доступа, не должно быть настроенных кодов с методом ввода *КОД#   "));
		}
	}
}
