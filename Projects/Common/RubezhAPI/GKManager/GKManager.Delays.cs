using RubezhAPI.GK;

namespace RubezhAPI
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
				x.OutputDependentElements.Remove(delay);
			});

			delay.OutputDependentElements.ForEach(x =>
			{
				x.InputDependentElements.Remove(delay);
				x.UpdateLogic(DeviceConfiguration);
				x.OnChanged();
			});

			delay.OnChanged();
		}

		/// <summary>
		/// Изменение задержки
		/// </summary>
		/// <param name="delay"></param>
		public static void EditDelay(GKDelay delay)
		{
			delay.OutputDependentElements.ForEach(x => x.OnChanged());
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