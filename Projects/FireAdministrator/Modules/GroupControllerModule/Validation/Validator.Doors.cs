using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Validation;
using System;
using Infrastructure.Common;
using FiresecAPI;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidateDoors()
		{
			ValidateDoorNoEquality();
			ValidateDoorSameDevices();

			foreach (var door in GKManager.DeviceConfiguration.Doors)
			{
				ValidateDoorHasNoDevices(door);
				ValidateDoorHasWrongDevices(door);
				ValidateLockProperties(door);
				ValidateLockControlDevice(door);
			}
		}

		void ValidateLockControlDevice(GKDoor door)
		{
			if (door.AntipassbackOn && door.DoorType != GKDoorType.Barrier)
			{
				if (door.LockControlDevice == null)
				{
					if (door.DoorType == GKDoorType.Turnstile)
						Errors.Add(new DoorValidationError(door, "При включенном Antipassback, отсутствует датчик проворота", ValidationErrorLevel.CannotWrite));
					else
						Errors.Add(new DoorValidationError(door, "При включенном Antipassback, отсутствует датчик контроля двери", ValidationErrorLevel.CannotWrite));
				}
				if (door.LockControlDeviceExit == null)
				{
					if (door.DoorType == GKDoorType.AirlockBooth)
						Errors.Add(new DoorValidationError(door, "При включенном Antipassback, отсутствует датчик контроля двери на выход", ValidationErrorLevel.CannotWrite));
				}
				if (door.EnterZoneUID == Guid.Empty)
					Errors.Add(new DoorValidationError(door, "При включенном Antipassback, отсутствует зона на вход", ValidationErrorLevel.CannotWrite));
				if (door.DoorType != GKDoorType.OneWay && door.ExitZoneUID == Guid.Empty)
					Errors.Add(new DoorValidationError(door, "При включенном Antipassback, отсутствует зона на выход", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateDoorNoEquality()
		{
			var doorNos = new HashSet<int>();
			foreach (var door in GKManager.DeviceConfiguration.Doors)
			{
				if (!doorNos.Add(door.No))
					Errors.Add(new DoorValidationError(door, "Дублируется номер", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateDoorSameDevices()
		{
			var deviceUIDs = new HashSet<Guid>();
			foreach (var door in GKManager.DeviceConfiguration.Doors)
			{
				var doorDeviceUIDs = new HashSet<Guid>();
				if (door.EnterButton != null)
				{
					if (!doorDeviceUIDs.Add(door.EnterButtonUID))
					{
						Errors.Add(new DoorValidationError(door, "Устройство " + door.EnterButton.PresentationName + " уже участвует в точке доступа", ValidationErrorLevel.CannotWrite));
					}
				}
				if (door.ExitButton != null)
				{
					if (!doorDeviceUIDs.Add(door.ExitButtonUID))
					{
						Errors.Add(new DoorValidationError(door, "Устройство " + door.ExitButton.PresentationName + " уже участвует в точке доступа", ValidationErrorLevel.CannotWrite));
					}
				}
				if (door.LockControlDeviceExit != null)
				{
					if (!doorDeviceUIDs.Add(door.LockControlDeviceExitUID))
					{
						Errors.Add(new DoorValidationError(door, "Устройство " + door.LockControlDeviceExit.PresentationName + " уже участвует в точке доступа", ValidationErrorLevel.CannotWrite));
					}
				}

				if (door.LockControlDevice != null)
				{
					if (!doorDeviceUIDs.Add(door.LockControlDevice.UID))
					{
						Errors.Add(new DoorValidationError(door, "Устройство " + door.LockControlDevice.PresentationName + " уже участвует в точке доступа", ValidationErrorLevel.CannotWrite));
					}
				}

				if (door.EnterDevice != null)
				{
					doorDeviceUIDs.Add(door.EnterDevice.UID);
				}
				if (door.ExitDevice != null)
				{
					doorDeviceUIDs.Add(door.ExitDevice.UID);
				}
				if (door.LockDevice != null)
				{
					doorDeviceUIDs.Add(door.LockDevice.UID);
				}

				foreach (var doorDeviceUID in doorDeviceUIDs)
				{
					if (!deviceUIDs.Add(doorDeviceUID))
					{
						var device = GKManager.Devices.FirstOrDefault(x => x.UID == doorDeviceUID);
						if (device != null)
						{
							Errors.Add(new DoorValidationError(door, "Устройство " + device.PresentationName + " уже участвует в другой точке доступа", ValidationErrorLevel.CannotWrite));
						}
					}
				}
			}
		}

		void ValidateDoorHasNoDevices(GKDoor door)
		{
			if (door.EnterDevice == null)
			{
				Errors.Add(new DoorValidationError(door, "К точке доступа не подключено устройство на вход", ValidationErrorLevel.CannotWrite));
			}
			if (door.DoorType == GKDoorType.TwoWay && door.ExitDevice == null)
			{
				Errors.Add(new DoorValidationError(door, "К точке доступа не подключено устройство на выход", ValidationErrorLevel.CannotWrite));
			}
			if (door.LockDevice == null)
			{
				if (door.DoorType == GKDoorType.AirlockBooth || door.DoorType == GKDoorType.Turnstile)
					Errors.Add(new DoorValidationError(door, "К точке доступа не подключено реле на вход", ValidationErrorLevel.CannotWrite));
				if (door.DoorType == GKDoorType.Barrier)
					Errors.Add(new DoorValidationError(door, "К точке доступа не подключено реле на открытие", ValidationErrorLevel.CannotWrite));
				Errors.Add(new DoorValidationError(door, "К точке доступа не подключен замок", ValidationErrorLevel.CannotWrite));
			}
			if (door.LockDeviceExit == null)
			{
				if (door.DoorType == GKDoorType.AirlockBooth || door.DoorType == GKDoorType.Turnstile)
					Errors.Add(new DoorValidationError(door, "К точке доступа не подключено реле на выход", ValidationErrorLevel.CannotWrite));
				if (door.DoorType == GKDoorType.Barrier)
					Errors.Add(new DoorValidationError(door, "К точке доступа не подключено реле на закрытие", ValidationErrorLevel.CannotWrite));
			}
			if (door.EnterButton == null)
			{
				if (door.DoorType == GKDoorType.AirlockBooth)
					Errors.Add(new DoorValidationError(door, "К точке доступа не подключена кнопка на вход", ValidationErrorLevel.CannotWrite));
			}
			if (door.ExitButton == null)
			{
				if (door.DoorType == GKDoorType.AirlockBooth)
					Errors.Add(new DoorValidationError(door, "К точке доступа не подключена кнопка на выход", ValidationErrorLevel.CannotWrite));
			}
			if (!GlobalSettingsHelper.GlobalSettings.IgnoredErrors.HasFlag(ValidationErrorType.SensorNotConnected))
			{
				if (door.LockControlDevice == null)
				{
					if (door.DoorType != GKDoorType.Barrier)
						Errors.Add(new DoorValidationError(door, "У точки доступа отсутствует датчик контроля двери на на вход", ValidationErrorLevel.Warning));
				}
				if (door.DoorType == GKDoorType.AirlockBooth)
				{
					if (door.LockControlDeviceExit == null)
						Errors.Add(new DoorValidationError(door, "У точки доступа отсутствует датчик контроля двери на выход", ValidationErrorLevel.Warning));
				}
			}
		}

		void ValidateDoorOtherLock(GKDoor door)
		{
			if (door.EnterDevice != null && door.ExitDevice != null && door.LockDevice != null)
			{
				if (door.LockDevice.DriverType == GKDriverType.RSR2_CardReader || door.LockDevice.DriverType == GKDriverType.RSR2_CardReader)
				{
					if (door.EnterDevice.UID != door.LockDevice.UID && door.ExitDevice.UID != door.LockDevice.UID)
						Errors.Add(new DoorValidationError(door, "Устройство Замок должно совпадать с устройством на вход или выход", ValidationErrorLevel.CannotWrite));
				}
			}
		}

		void ValidateDoorHasWrongDevices(GKDoor door)
		{
			if (door.EnterDevice != null && door.EnterDevice.DriverType != GKDriverType.RSR2_CodeReader && door.EnterDevice.DriverType != GKDriverType.RSR2_CardReader)
			{
				Errors.Add(new DoorValidationError(door, "К точке доступа подключено неверное устройство на вход", ValidationErrorLevel.CannotWrite));
			}

			if (door.ExitDevice != null)
			{
				if (door.DoorType == GKDoorType.OneWay && door.ExitDevice.DriverType != GKDriverType.RSR2_AM_1)
				{
					Errors.Add(new DoorValidationError(door, "К точке доступа подключено неверное устройство на выход", ValidationErrorLevel.CannotWrite));
				}
				if (door.DoorType == GKDoorType.TwoWay && door.ExitDevice.DriverType != GKDriverType.RSR2_CodeReader && door.ExitDevice.DriverType != GKDriverType.RSR2_CardReader)
				{
					Errors.Add(new DoorValidationError(door, "К точке доступа подключено неверное устройство на выход", ValidationErrorLevel.CannotWrite));
				}
			}

			if (door.LockDevice != null && door.LockDevice.DriverType != GKDriverType.RSR2_RM_1 && door.LockDevice.DriverType != GKDriverType.RSR2_MVK8 && door.LockDevice.DriverType != GKDriverType.RSR2_CodeReader && door.LockDevice.DriverType != GKDriverType.RSR2_CardReader)
			{
				Errors.Add(new DoorValidationError(door, "К точке доступа подключено неверное устройство на замок", ValidationErrorLevel.CannotWrite));
			}

			if (door.LockControlDevice != null && door.LockControlDevice.DriverType != GKDriverType.RSR2_AM_1)
			{
				Errors.Add(new DoorValidationError(door, "К точке доступа подключено неверное устройство на датчик контроля двери", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateLockLogic(GKDoor door)
		{
			if (door.LockDevice != null)
			{
				if (door.LockDevice.Logic.GetObjects().Count > 0)
				{
					Errors.Add(new DoorValidationError(door, "Устройство Замок не должно иметь настроенную логику срабатывания", ValidationErrorLevel.CannotWrite));
				}
			}
		}

		void ValidateLockProperties(GKDoor door)
		{
			if (door.LockDevice != null)
			{
				switch (door.LockDevice.DriverType)
				{
					case GKDriverType.RSR2_RM_1:
					case GKDriverType.RSR2_MVK8:
					case GKDriverType.RSR2_CodeReader:
					case GKDriverType.RSR2_CardReader:
						if (door.LockDevice.Properties.FirstOrDefault(x => x.Name == "Состояние контакта для режима Выключено").Value != 0)
							Errors.Add(new DeviceValidationError(door.LockDevice, "Парамер 'Состояние контакта для режима Выключено' устройства, участвующего в ТД, должен быть 'Контакт НР'", ValidationErrorLevel.CannotWrite));
						if (door.LockDevice.Properties.FirstOrDefault(x => x.Name == "Состояние контакта для режима Удержания").Value != 4)
							Errors.Add(new DeviceValidationError(door.LockDevice, "Парамер 'Состояние контакта для режима Удержания' устройства, участвующего в ТД, должен быть 'Контакт НЗ'", ValidationErrorLevel.CannotWrite));
						if (door.LockDevice.Properties.FirstOrDefault(x => x.Name == "Состояние контакта для режима Включено").Value != 16)
							Errors.Add(new DeviceValidationError(door.LockDevice, "Парамер 'Состояние контакта для режима Включено' устройства, участвующего в ТД, должен быть 'Контакт НЗ'", ValidationErrorLevel.CannotWrite));
						break;
				}

				switch (door.LockDevice.DriverType)
				{
					case GKDriverType.RSR2_RM_1:
					case GKDriverType.RSR2_MVK8:
						if (door.LockDevice.Properties.FirstOrDefault(x => x.Name == "Задержка на включение, с").Value != 0)
							Errors.Add(new DeviceValidationError(door.LockDevice, "Парамер 'Задержка на включение, с' устройства, участвующего в ТД, должен быть '0'", ValidationErrorLevel.CannotWrite));
						break;

					case GKDriverType.RSR2_CodeReader:
					case GKDriverType.RSR2_CardReader:
						if (door.LockDevice.Properties.FirstOrDefault(x => x.Name == "Задержка на включение, с").Value != 0)
							Errors.Add(new DeviceValidationError(door.LockDevice, "Парамер 'Задержка на включение, с' устройства, участвующего в ТД, должен быть '0'", ValidationErrorLevel.CannotWrite));
						break;
				}

				switch (door.LockDevice.DriverType)
				{
					case GKDriverType.RSR2_CodeReader:
					case GKDriverType.RSR2_CardReader:
						if (door.LockDevice.Properties.FirstOrDefault(x => x.Name == "Наличие реле").Value != 2)
							Errors.Add(new DeviceValidationError(door.LockDevice, "Парамер 'Наличие реле' устройства, участвующего в ТД, должен быть 'Есть'", ValidationErrorLevel.CannotWrite));
						break;
				}
			}
		}
	}
}