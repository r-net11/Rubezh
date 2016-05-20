using Infrastructure.Common;
using Infrastructure.Common.Validation;
using RubezhAPI;
using RubezhAPI.GK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidateDoors()
		{
			ValidateCommon(GKManager.Doors);
			ValidateDoorSameDevices();

			foreach (var door in GKManager.Doors)
			{
				ValidateDoorHasNoDevices(door);
				ValidateDoorHasWrongDevices(door);
				//ValidateLockControlDevice(door);
				ValidateLockProperties(door);
				ValidateLockLogic(door);
                ValidateZoneDifference(door);
                
            }
		}

		/// <summary>
		/// Валидация того, что в рамках каждой ТД не может быть повторяющихся устройства
		/// Исключение делается для замка на вход и замка на выход. Устройства для замков могут совпадать с устройствами на вход или выход
		/// Также устройства, уже учавствующие в ТД, не могут учавствовать в других ТД
		/// </summary>
		void ValidateDoorSameDevices()
		{
			var deviceUIDs = new HashSet<Guid>();
			foreach (var door in GKManager.DeviceConfiguration.Doors)
			{
				var doorDeviceUIDs = new HashSet<Guid>();

				if (door.EnterDevice != null)
				{
					if (!doorDeviceUIDs.Add(door.EnterDevice.UID))
						AddError(door, "Устройство " + door.EnterDevice.PresentationName + " может учасвствовать только в устройстве на вход или в устройстве на выход", ValidationErrorLevel.CannotWrite);
				}
				if (door.ExitDevice != null)
				{
					if (!doorDeviceUIDs.Add(door.ExitDevice.UID))
						AddError(door, "Устройство " + door.ExitDevice.PresentationName + " не может быть одновременно устройством на вход и устройством на выход", ValidationErrorLevel.CannotWrite);
				}
				if (door.LockDevice != null)
				{
					doorDeviceUIDs.Add(door.LockDevice.UID);
				}

				if (door.LockDeviceExit != null)
				{
					doorDeviceUIDs.Add(door.LockDeviceExit.UID);
					if (door.LockDevice != null && door.LockDeviceExit.UID.Equals(door.LockDevice.UID))
						if (door.DoorType == GKDoorType.Barrier)
							AddError(door, "Устройство " + door.LockDeviceExit.PresentationName + " не может быть одновременно реле на открытие и реле на закрытие", ValidationErrorLevel.CannotWrite);
						else
							AddError(door, "Устройство " + door.LockDeviceExit.PresentationName + " не может быть одновременно реле на вход и реле на выход", ValidationErrorLevel.CannotWrite);
				}
				if (door.EnterButton != null)
				{
					if (!doorDeviceUIDs.Add(door.EnterButtonUID))
						AddError(door, "Устройство " + door.EnterButton.PresentationName + " уже участвует в точке доступа", ValidationErrorLevel.CannotWrite);
				}
				if (door.ExitButton != null)
				{
					if (!doorDeviceUIDs.Add(door.ExitButtonUID))
						AddError(door, "Устройство " + door.ExitButton.PresentationName + " уже участвует в точке доступа", ValidationErrorLevel.CannotWrite);
				}
				if (door.LockControlDeviceExit != null)
				{
					if (!doorDeviceUIDs.Add(door.LockControlDeviceExitUID))
						AddError(door, "Устройство " + door.LockControlDeviceExit.PresentationName + " уже участвует в точке доступа", ValidationErrorLevel.CannotWrite);
				}
				if (door.LockControlDevice != null)
				{
					if (!doorDeviceUIDs.Add(door.LockControlDevice.UID))
						AddError(door, "Устройство " + door.LockControlDevice.PresentationName + " уже участвует в точке доступа", ValidationErrorLevel.CannotWrite);
				}

				foreach (var doorDeviceUID in doorDeviceUIDs)
				{
					if (!deviceUIDs.Add(doorDeviceUID))
					{
						var device = GKManager.Devices.FirstOrDefault(x => x.UID == doorDeviceUID);
						if (device != null)
						{
							AddError(door, "Устройство " + device.PresentationName + " уже участвует в другой точке доступа", ValidationErrorLevel.CannotWrite);
						}
					}
				}
			}
		}

		void ValidateLockControlDevice(GKDoor door)
		{
			if (door.AntipassbackOn)
			{
				if (door.LockControlDevice == null)
				{
					if (door.DoorType == GKDoorType.Turnstile)
						AddError(door, "При включенном Antipassback, отсутствует датчик проворота", ValidationErrorLevel.CannotWrite);
					else if (door.DoorType == GKDoorType.Barrier)
						AddError(door, "При включенном Antipassback, отсутствует датчик контроля въезда", ValidationErrorLevel.CannotWrite);
					else
						AddError(door, "При включенном Antipassback, отсутствует датчик контроля двери", ValidationErrorLevel.CannotWrite);

				}
				if (door.LockControlDeviceExit == null)
				{
					if (door.DoorType == GKDoorType.AirlockBooth)
						AddError(door, "При включенном Antipassback, отсутствует датчик контроля двери на выход", ValidationErrorLevel.CannotWrite);
					if (door.DoorType == GKDoorType.Barrier)
						AddError(door, "При включенном Antipassback, отсутствует датчик контроля выезда", ValidationErrorLevel.CannotWrite);
				}

				if (door.EnterZoneUID == Guid.Empty)
					AddError(door, "При включенном Antipassback, отсутствует зона на вход", ValidationErrorLevel.CannotWrite);
				if (door.DoorType != GKDoorType.OneWay && door.ExitZoneUID == Guid.Empty)
					AddError(door, "При включенном Antipassback, отсутствует зона на выход", ValidationErrorLevel.CannotWrite);
			}
			else
			{
				if (door.LockControlDevice == null)
				{
					 if (door.DoorType == GKDoorType.Barrier)
						 AddError(door, "Для шлагбаума должен быть задан датчик контроля на въезд", ValidationErrorLevel.CannotWrite);
				}
				if (door.LockControlDeviceExit == null)
				{
					if (door.DoorType == GKDoorType.Barrier)
						AddError(door, "Для шлагбаума должен быть задан датчик контроля на выезд", ValidationErrorLevel.CannotWrite);
				}

				if (!GlobalSettingsHelper.GlobalSettings.IgnoredErrors.Contains(ValidationErrorType.SensorNotConnected))
				{
					if (door.LockControlDevice == null)
					{
						if (door.DoorType == GKDoorType.Turnstile)
							AddError(door, "У точки доступа отсутствует датчик проворота", ValidationErrorLevel.Warning);
						if (door.DoorType != GKDoorType.Barrier && door.DoorType != GKDoorType.Turnstile)
							AddError(door, "У точки доступа отсутствует датчик контроля двери", ValidationErrorLevel.Warning);

					}
					if (door.LockControlDeviceExit == null)
					{
						if (door.DoorType == GKDoorType.AirlockBooth)
							AddError(door, "У точки доступа отсутствует датчик контроля двери на выход", ValidationErrorLevel.Warning);
					}
				}
			}
		}

		/// <summary>
		/// Валидация того, что ТД данного типа подключены все необходимые устройства
		/// </summary>
		/// <param name="door"></param>
		void ValidateDoorHasNoDevices(GKDoor door)
		{
			if (door.EnterDevice == null)
			{
				AddError(door, "К точке доступа не подключено устройство на вход", ValidationErrorLevel.CannotWrite);
			}
			if (door.ExitDevice == null)
			{
				AddError(door, "К точке доступа не подключено устройство на выход", ValidationErrorLevel.CannotWrite);
			}
			if (door.LockDevice == null)
			{
				if (door.DoorType == GKDoorType.AirlockBooth || door.DoorType == GKDoorType.Turnstile)
					AddError(door, "К точке доступа не подключено реле на вход", ValidationErrorLevel.CannotWrite);
				else if (door.DoorType == GKDoorType.Barrier)
					AddError(door, "К точке доступа не подключено реле на открытие", ValidationErrorLevel.CannotWrite);
				else
					AddError(door, "К точке доступа не подключен замок", ValidationErrorLevel.CannotWrite);
			}
			if (door.LockDeviceExit == null)
			{
				if (door.DoorType == GKDoorType.AirlockBooth || door.DoorType == GKDoorType.Turnstile)
					AddError(door, "К точке доступа не подключено реле на выход", ValidationErrorLevel.CannotWrite);
				if (door.DoorType == GKDoorType.Barrier)
					AddError(door, "К точке доступа не подключено реле на закрытие", ValidationErrorLevel.CannotWrite);
			}
			if (door.EnterButton == null)
			{
				if (door.DoorType == GKDoorType.AirlockBooth)
					AddError(door, "К точке доступа не подключена кнопка на вход", ValidationErrorLevel.CannotWrite);
			}
			if (door.ExitButton == null)
			{
				if (door.DoorType == GKDoorType.AirlockBooth)
					AddError(door, "К точке доступа не подключена кнопка на выход", ValidationErrorLevel.CannotWrite);
			}

			ValidateLockControlDevice(door);
		}

		void ValidateDoorOtherLock(GKDoor door)
		{
			if (door.EnterDevice != null && door.ExitDevice != null && door.LockDevice != null)
			{
				if (door.LockDevice.Driver.IsCardReaderOrCodeReader)
				{
					if (door.EnterDevice.UID != door.LockDevice.UID && door.ExitDevice.UID != door.LockDevice.UID)
						AddError(door, "Устройство Замок должно совпадать с устройством на вход или выход", ValidationErrorLevel.CannotWrite);
				}
			}
		}

		void ValidateDoorHasWrongDevices(GKDoor door)
		{
			if (door.EnterDevice != null && !door.EnterDevice.Driver.IsCardReaderOrCodeReader)
			{
				AddError(door, "К точке доступа подключено неверное устройство на вход", ValidationErrorLevel.CannotWrite);
			}

			if (door.ExitDevice != null)
			{
				if (door.DoorType == GKDoorType.OneWay && door.ExitDevice.DriverType != GKDriverType.RSR2_AM_1)
				{
					AddError(door, "К точке доступа подключено неверное устройство на выход", ValidationErrorLevel.CannotWrite);
				}
				if (door.DoorType == GKDoorType.TwoWay && !door.ExitDevice.Driver.IsCardReaderOrCodeReader)
				{
					AddError(door, "К точке доступа подключено неверное устройство на выход", ValidationErrorLevel.CannotWrite);
				}
			}

			if (door.LockDevice != null && door.LockDevice.DriverType != GKDriverType.RSR2_RM_1 && door.LockDevice.DriverType != GKDriverType.RSR2_MVK8 && !door.LockDevice.Driver.IsCardReaderOrCodeReader)
			{
				AddError(door, "К точке доступа подключено неверное устройство на замок", ValidationErrorLevel.CannotWrite);
			}

			if (door.LockControlDevice != null && door.LockControlDevice.DriverType != GKDriverType.RSR2_AM_1)
			{
				AddError(door, "К точке доступа подключено неверное устройство на датчик контроля двери", ValidationErrorLevel.CannotWrite);
			}
		}

		/// <summary>
		/// Валидация того, что устройства, учавствующие в ТД, имеют правильно заданные параметры
		/// </summary>
		/// <param name="door"></param>
		void ValidateLockProperties(GKDoor door)
		{
			if (door.LockDevice != null)
			{
				switch (door.LockDevice.DriverType)
				{
					case GKDriverType.RSR2_RM_1:
					case GKDriverType.RSR2_MVK8:
						if (door.LockDevice.Properties.FirstOrDefault(x => x.Name == "Задержка на включение, с").Value != 0)
							AddError(door.LockDevice, "Парамер 'Задержка на включение, с' устройства, участвующего в ТД, должен быть '0'", ValidationErrorLevel.CannotWrite);
						break;

					case GKDriverType.RSR2_CodeReader:
					case GKDriverType.RSR2_CardReader:
					case GKDriverType.RSR2_CodeCardReader:
						if (door.LockDevice.Properties.FirstOrDefault(x => x.Name == "Задержка на включение, с").Value != 0)
							AddError(door.LockDevice, "Парамер 'Задержка на включение, с' устройства, участвующего в ТД, должен быть '0'", ValidationErrorLevel.CannotWrite);
						break;
				}

				switch (door.LockDevice.DriverType)
				{
					case GKDriverType.RSR2_CodeReader:
					case GKDriverType.RSR2_CardReader:
					case GKDriverType.RSR2_CodeCardReader:
						if (door.LockDevice.Properties.FirstOrDefault(x => x.Name == "Наличие реле").Value != 2)
							AddError(door.LockDevice, "Парамер 'Наличие реле' устройства, участвующего в ТД, должен быть 'Есть'", ValidationErrorLevel.CannotWrite);
						break;
				}
			}
		}

		/// <summary>
		/// Валидация того, что устройство замок не должно иметь настроенной логики срабатывания
		/// </summary>
		/// <param name="door"></param>
		void ValidateLockLogic(GKDoor door)
		{
			if (door.LockDevice != null)
			{
				if (door.LockDevice.Logic.GetObjects().Count > 0)
				{
					AddError(door, "Устройство Замок не должно иметь настроенную логику срабатывания", ValidationErrorLevel.CannotWrite);
				}
			}

			if (door.LockDeviceExit != null)
			{
				if (door.LockDeviceExit.Logic.GetObjects().Count > 0)
				{
					AddError(door, "Устройство Замок не должно иметь настроенную логику срабатывания", ValidationErrorLevel.CannotWrite);
				}
			}
		}

		void ValidateZoneDifference(GKDoor door)
		{
			if ((door.EnterZoneUID == door.ExitZoneUID)&&(door.EnterZoneUID != Guid.Empty) && (door.ExitZoneUID != Guid.Empty))
			{
				AddError(door, "В точке доступа зона входа и зона выхода должны быть разными", ValidationErrorLevel.CannotWrite);
			}
		}

	}
}