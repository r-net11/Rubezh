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
		/// Добавление зоны
		/// </summary>
		/// <param name="zone"></param>
		public static void AddZone(GKZone zone)
		{
			Zones.Add(zone);
		}

		/// <summary>
		/// Удаление зоны
		/// </summary>
		/// <param name="zone"></param>
		public static void RemoveZone(GKZone zone)
		{
			Zones.Remove(zone);
			zone.OutputDependentElements.ForEach(x =>
			{
				x.InputDependentElements.Remove(zone);
				if (x is GKDevice)
				{
					x.Invalidate(GKManager.DeviceConfiguration);
					x.OnChanged();
				}
				x.UpdateLogic(GKManager.DeviceConfiguration);
				x.OnChanged();
			});

			foreach (var device in zone.Devices)
			{
				device.Zones.Remove(zone);
				device.ZoneUIDs.Remove(zone.UID);
				device.OnChanged();
			}

			zone.OnChanged();
		}

		/// <summary>
		/// Редактирование зоны
		/// </summary>
		/// <param name="zone"></param>
		public static void EditZone(GKZone zone)
		{
			zone.OnChanged();
			zone.OutputDependentElements.ForEach(x => x.OnChanged());
		}
	}
}