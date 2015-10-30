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
		/// Добавление задержки
		/// </summary>
		/// <param name="delay"></param>
		public static void AddDelay(GKDelay delay)
		{
			Delays.Add(delay);
		}

		/// <summary>
		/// Удаление задержки
		/// </summary>
		/// <param name="delay"></param>
		public static void RemoveDelay(GKDelay delay)
		{
			Delays.Remove(delay);
			delay.InputDependentElements.ForEach(x =>
			{
				x.OutDependentElements.Remove(delay);
			});

			delay.OutDependentElements.ForEach(x =>
			{
				x.InputDependentElements.Remove(delay);
				x.UpdateLogic();
				x.OnChanged();
			});
		}

		/// <summary>
		/// Изменение задержки
		/// </summary>
		/// <param name="delay"></param>
		public static void EditDelay(GKDelay delay)
		{
			delay.OutDependentElements.ForEach(x => x.OnChanged());
			delay.OnChanged();
		}

		/// <summary>
		/// Изменение логики задержки
		/// </summary>
		/// <param name="delay"></param>
		/// <param name="logic"></param>
		public static void SetDelayLogic(GKDelay delay, GKLogic logic)
		{
			delay.Logic = logic;
			delay.ChangedLogic();
		}
	}
}