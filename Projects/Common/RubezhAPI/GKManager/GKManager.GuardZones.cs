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
		/// Добавление охранной зоны
		/// </summary>
		/// <param name="guardZone"></param>
		public static void AddGuardZone(GKGuardZone guardZone)
		{
			GuardZones.Add(guardZone);
		}

		/// <summary>
		/// Удаление охранной зоны
		/// </summary>
		/// <param name="guardZone"></param>
		public static void RemoveGuardZone(GKGuardZone guardZone)
		{
			GuardZones.Remove(guardZone);
			guardZone.OnChanged();
			guardZone.OutDependentElements.ForEach(x =>
			{
				x.InputDependentElements.Remove(guardZone);
				if (x is GKDevice)
				{
					x.Invalidate();
					x.OnChanged();
				}
				x.UpdateLogic();
				x.OnChanged();
			});
		}

		/// <summary>
		/// Изменение охранной зоны
		/// </summary>
		/// <param name="guardZone"></param>
		public static void EditGuardZone(GKGuardZone guardZone)
		{
			guardZone.InputDependentElements.ForEach(x => x.OnChanged());
			guardZone.OutDependentElements.ForEach(x => x.OnChanged());
			guardZone.OnChanged();
		}
	}
}