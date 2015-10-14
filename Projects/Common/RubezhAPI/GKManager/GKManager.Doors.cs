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
			door.InputDependentElements.ForEach(x =>
			{
				x.OutDependentElements.Remove(door);
			});

			door.OutDependentElements.ForEach(x =>
			{
				x.InputDependentElements.Remove(door);
				x.UpdateLogic();
				x.OnChanged();
			});
			door.OnChanged();
		}
	}
}