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
	}
}