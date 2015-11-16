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
		/// Добавление направления
		/// </summary>
		/// <param name="direction"></param>
		public static void AddDirection(GKDirection direction)
		{
			Directions.Add(direction);
		}

		/// <summary>
		/// Удаление направления
		/// </summary>
		/// <param name="direction"></param>
		public static void RemoveDirection(GKDirection direction)
		{
			Directions.Remove(direction);
			direction.InputDependentElements.ForEach(x =>
			{
				x.OutputDependentElements.Remove(direction);
			});

			direction.OutputDependentElements.ForEach(x =>
			{
				x.InputDependentElements.Remove(direction);
				x.UpdateLogic(GKManager.DeviceConfiguration);
				x.OnChanged();
			});
			direction.OnChanged();
		}

		/// <summary>
		/// Изменение направления
		/// </summary>
		/// <param name="direction"></param>
		public static void EditDirection(GKDirection direction)
		{
			direction.OutputDependentElements.ForEach(x => x.OnChanged());
			direction.OnChanged();
		}

		/// <summary>
		/// Изменение логики направления
		/// </summary>
		/// <param name="direction"></param>
		/// <param name="logic"></param>
		public static void SetDirectionLogic(GKDirection direction, GKLogic logic)
		{
			direction.Logic = logic;
			direction.ChangedLogic();
		}
	}
}