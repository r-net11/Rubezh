using System;
using System.Collections.Generic;
using System.Linq;
using GKModule.Validation;
using Infrastructure.Common.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RubezhAPI.GK;
using RubezhClient;

namespace GKProcessor.Test
{
	public partial class ValidationTest
	{
		[TestMethod]
		public void TestDoorHasNoDevices()
		{
			foreach (var doorType in new List<GKDoorType>(Enum.GetValues(typeof(GKDoorType)).Cast<GKDoorType>()))
			{
				var door = new GKDoor {DoorType = doorType};
				GKManager.Doors.Add(door);
				var validator = new Validator();
				var errors = validator.Validate();
				if (doorType == GKDoorType.Barrier)
				{ 
					Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "Для шлагбаума должен быть задан датчик контроля на въезд"));
					Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "Для шлагбаума должен быть задан датчик контроля на выезд"));
					Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено реле на открытие"));
					Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено реле на закрытие"));
				}
				else if (door.DoorType == GKDoorType.AirlockBooth || door.DoorType == GKDoorType.Turnstile)
				{
					Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено реле на вход"));
					Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено реле на выход"));
					if (door.DoorType == GKDoorType.AirlockBooth)
					{
						Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключена кнопка на вход"));
						Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключена кнопка на выход"));
					}
				}
				else
				{
					Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключен замок"));
				}

				Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено устройство на вход"));
				Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено устройство на выход"));

				door.AntipassbackOn = true;
				validator = new Validator();
				errors = validator.Validate();
				if (door.DoorType == GKDoorType.Turnstile)
					Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "При включенном Antipassback, отсутствует датчик проворота"));
				else
					Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "При включенном Antipassback, отсутствует датчик контроля двери"));
				if (door.DoorType == GKDoorType.AirlockBooth)
					Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "При включенном Antipassback, отсутствует датчик контроля двери на выход"));
				if (door.EnterZoneUID == Guid.Empty)
					Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "При включенном Antipassback, отсутствует зона на вход"));
				if (door.DoorType != GKDoorType.OneWay && door.ExitZoneUID == Guid.Empty)
					Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "При включенном Antipassback, отсутствует зона на выход"));
			}
		}
	}
}
