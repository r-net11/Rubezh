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
		/// Добавление НС
		/// </summary>
		/// <param name="pumpStation"></param>
		public static void AddPumpStation(GKPumpStation pumpStation)
		{
			PumpStations.Add(pumpStation);
		}

		/// <summary>
		/// Удаление НС
		/// </summary>
		/// <param name="pumpStation"></param>
		public static void RemovePumpStation(GKPumpStation pumpStation)
		{
			PumpStations.Remove(pumpStation);
			pumpStation.InputDependentElements.ForEach(x =>
			{
				x.OutDependentElements.Remove(pumpStation);
			});

			pumpStation.OutDependentElements.ForEach(x =>
			{
				x.InputDependentElements.Remove(pumpStation);
				x.UpdateLogic();
				x.OnChanged();
			});
		}

		/// <summary>
		/// Изменение НС
		/// </summary>
		/// <param name="pumpStation"></param>
		public static void EditPumpStation(GKPumpStation pumpStation)
		{
			pumpStation.OutDependentElements.ForEach(x => x.OnChanged());
		}
	}
}