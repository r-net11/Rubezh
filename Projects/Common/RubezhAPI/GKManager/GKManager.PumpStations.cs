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
				x.OutputDependentElements.Remove(pumpStation);
			});

			pumpStation.OutputDependentElements.ForEach(x =>
			{
				x.InputDependentElements.Remove(pumpStation);
				x.UpdateLogic(GKManager.DeviceConfiguration);
				x.OnChanged();
			});
		}

		/// <summary>
		/// Изменение НС
		/// </summary>
		/// <param name="pumpStation"></param>
		public static void EditPumpStation(GKPumpStation pumpStation)
		{
			pumpStation.OutputDependentElements.ForEach(x => x.OnChanged());
			pumpStation.OnChanged();
		}
	}
}