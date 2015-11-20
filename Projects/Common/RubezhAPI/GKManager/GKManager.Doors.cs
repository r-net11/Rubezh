using RubezhAPI.GK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RubezhClient
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
				x.UpdateLogic(GKManager.DeviceConfiguration);
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

		public static void ChangeLockControlDevice(GKDoor door, GKDevice device)
		{
			RemoveDependenctElement(door, door.LockControlDeviceUID);

			door.LockControlDeviceUID = device != null ? device.UID : Guid.Empty;
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

		public static void ChangeLockControlDeviceExit(GKDoor door, GKDevice device)
		{
			RemoveDependenctElement(door, door.LockControlDeviceExitUID);

			door.LockControlDeviceExitUID = device != null ? device.UID : Guid.Empty;
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

		public static Tuple <Guid, GKDevice> ChangeDevice(GKDoor door, Tuple <Guid, GKDevice> tuple , GKDevice device)
		{
			var _deviceUID = tuple.Item1;
			var _device = tuple.Item2;
			RemoveDependenctElement(door, tuple.Item1);

			_deviceUID = device != null ? device.UID : Guid.Empty;

			if (_device != null)
			{
				_device.Door = null;
			}
			_device = device;
			if (_device != null)
			{
				device.Door = door;
				door.AddDependentElement(device);
			}
			door.OnChanged();

			return new Tuple<Guid, GKDevice>(_deviceUID, _device);
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