using RubezhAPI.GK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RubezhAPI
{
	public partial class GKManager
	{
		/// <summary>
		/// Добавление ТД
		/// </summary>
		/// <param name="door"></param>
		public static void AddDoor(GKDoor door)
		{
			Doors.Add(door);
		}

		/// <summary>
		/// Удаление ТД
		/// </summary>
		/// <param name="door"></param>
		public static void RemoveDoor(GKDoor door)
		{
			Doors.Remove(door);
			Devices.Where(x => x.UID == door.EnterDeviceUID || x.UID == door.ExitDeviceUID || x.UID == door.LockDeviceUID || x.UID == door.LockControlDeviceUID).ToList().ForEach(x => x.Door = null);
			door.InputDependentElements.ForEach(x =>
			{
				x.OutputDependentElements.Remove(door);
			});

			door.OutputDependentElements.ForEach(x =>
			{
				x.InputDependentElements.Remove(door);
				x.UpdateLogic(DeviceConfiguration);
				x.OnChanged();
			});
			door.OnChanged();
		}

		/// <summary>
		/// Изменение ТД
		/// </summary>
		/// <param name="door"></param>
		public static void EditDoor(GKDoor door)
		{
			door.OnChanged();
			door.OutputDependentElements.ForEach(x => x.OnChanged());
			door.InputDependentElements.ForEach(x => x.OnChanged());
		}

		public static void SetDoorOpenRegimeLogic(GKDoor door, GKLogic newLogic)
		{
			door.OpenRegimeLogic = newLogic;
			door.ChangedLogic();
		}
		
		public static void SetDoorOpenExitRegimeLogic(GKDoor door, GKLogic newLogic)
		{
			door.OpenExitRegimeLogic = newLogic;
			door.ChangedLogic();
		}

		public static void SetDoorNormRegimeLogic(GKDoor door, GKLogic newLogic)
		{
			door.NormRegimeLogic = newLogic;
			door.ChangedLogic();
		}

		public static void SetDoorCloseRegimeLogic(GKDoor door, GKLogic newLogic)
		{
			door.CloseRegimeLogic = newLogic;
			door.ChangedLogic();
		}

		public static void ChangeEnterButtonDevice(GKDoor door, GKDevice device)
		{
			RemoveDependenctElement(door, door.EnterButtonUID);

			door.EnterButtonUID = device != null ? device.UID : Guid.Empty;
			if (door.EnterButton != null)
			{
				door.EnterButton.Door = null;
			}
			door.EnterButton = device;
			if (door.EnterButton != null)
			{
				door.EnterButton.Door = door;
				door.AddDependentElement(door.EnterButton);
			}
			door.OnChanged();
		}

		public static void ChangeEnterDevice(GKDoor door, GKDevice device)
		{
			RemoveDependenctElement(door, door.EnterDeviceUID);

			door.EnterDeviceUID = device != null ? device.UID : Guid.Empty;
			if (door.EnterDevice != null)
			{
				door.EnterDevice.Door = null;
			}
			door.EnterDevice = device;
			if (door.EnterDevice != null)
			{
				door.EnterDevice.Door = door;
				door.AddDependentElement(door.EnterDevice);
			}
			door.OnChanged();
		}

		public static void ChangeExitDevice(GKDoor door, GKDevice device)
		{
			RemoveDependenctElement(door, door.ExitDeviceUID);

			door.ExitDeviceUID = device != null ? device.UID : Guid.Empty;
			if (door.ExitDevice != null)
			{
				door.ExitDevice.Door = null;
			}
			door.ExitDevice = device;
			if (door.ExitDevice != null)
			{
				door.ExitDevice.Door = door;
				door.AddDependentElement(door.ExitDevice);
			}
			door.OnChanged();
		}

		public static void ChangeExitButtonDevice(GKDoor door, GKDevice device)
		{
			RemoveDependenctElement(door, door.ExitButtonUID);

			door.ExitButtonUID = device != null ? device.UID : Guid.Empty;
			if (door.ExitButton != null)
			{
				door.ExitButton.Door = null;
			}
			door.ExitButton = device;
			if (door.ExitButton != null)
			{
				door.ExitButton.Door = door;
				door.AddDependentElement(door.ExitButton);
			}
			door.OnChanged();
		}

		public static void ChangeLockDevice(GKDoor door, GKDevice device)
		{
			RemoveDependenctElement(door, door.LockDeviceUID);

			door.LockDeviceUID = device != null ? device.UID : Guid.Empty;
			if (door.LockDevice != null)
			{
				door.LockDevice.Door = null;
			}
			door.LockDevice = device;
			if (door.LockDevice != null)
			{
				door.LockDevice.Door = door;
				door.AddDependentElement(door.LockDevice);
			}
			door.OnChanged();
		}

		public static void ChangeLockDeviceExit(GKDoor door, GKDevice device)
		{
			RemoveDependenctElement(door, door.LockDeviceExitUID);

			door.LockDeviceExitUID = device != null ? device.UID : Guid.Empty;
			if (door.LockDeviceExit != null)
			{
				door.LockDeviceExit.Door = null;
			}
			door.LockDeviceExit = device;
			if (door.LockDeviceExit != null)
			{
				door.LockDeviceExit.Door = door;
				door.AddDependentElement(door.LockDeviceExit);
			}
			door.OnChanged();
		}

		public static void ChangeResetDevice(GKDoor door, GKDevice device)
		{
			RemoveDependenctElement(door, door.ResetDeviceUID);

			door.ResetDeviceUID = device != null ? device.UID : Guid.Empty;
			if (door.ResetDevice != null)
			{
				door.ResetDevice.Door = null;
			}
			door.ResetDevice = device;
			if (door.ResetDevice != null)
			{
				door.ResetDevice.Door = door;
				door.AddDependentElement(door.ResetDevice);
			}
			door.OnChanged();
		}

		public static void ChangeLockControlDevice(GKDoor door, GKDevice device)
		{
			RemoveDependenctElement(door, door.LockControlDeviceUID);

			door.LockControlDeviceUID = device != null ? device.UID : Guid.Empty;
			if (door.LockControlDevice != null)
			{
				door.LockControlDevice.Door = null;
			}
			door.LockControlDevice = device;
			if (door.LockControlDevice != null)
			{
				door.LockControlDevice.Door = door;
				door.AddDependentElement(door.LockControlDevice);
			}
			door.OnChanged();
		}

		public static void ChangeLockControlDeviceExit(GKDoor door,  GKDevice device)
		{
			RemoveDependenctElement(door, door.LockControlDeviceExitUID);

			door.LockControlDeviceExitUID = device != null ? device.UID : Guid.Empty;
			if (door.LockControlDeviceExit != null)
			{
				door.LockControlDeviceExit.Door = null;
			}
			door.LockControlDeviceExit = device;
			if (door.LockControlDeviceExit != null)
			{
				door.LockControlDeviceExit.Door = door;
				door.AddDependentElement(door.LockControlDeviceExit);
			}
			door.OnChanged();
		}

		public static void ChangeEnterZone(GKDoor door, GKSKDZone zone)
		{
			door.EnterZoneUID = zone != null ? zone.UID : Guid.Empty;
			door.OnChanged();
		}

		public static void ChangeExitZone(GKDoor door, GKSKDZone zone)
		{
			door.ExitZoneUID = zone != null ? zone.UID : Guid.Empty;
			door.OnChanged();
		}

		public static void RemoveDependenctElement(GKDoor door, Guid deviceUID)
		{
			if (deviceUID != Guid.Empty)
			{
				door.InputDependentElements.ForEach(x =>
				{
					if (x.UID == deviceUID)
						x.OutputDependentElements.Remove(door);
				});
				door.InputDependentElements.RemoveAll(x => x.UID == deviceUID);
			}
		}
	}
}