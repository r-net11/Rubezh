using RubezhAPI.GK;
using System;
using System.Collections.Generic;

namespace RubezhAPI
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

		public static void SetPumpStationStartLogic(GKPumpStation pumpStation, GKLogic newLogic)
		{
			pumpStation.StartLogic = newLogic;
			pumpStation.ChangedLogic();
		}

		public static void SetPumpStationStopLogic(GKPumpStation pumpStation, GKLogic newLogic)
		{
			pumpStation.StopLogic = newLogic;
			pumpStation.ChangedLogic();
		}

		public static void SetPumpStationAutomaticOffLogic(GKPumpStation pumpStation, GKLogic newLogic)
		{
			pumpStation.AutomaticOffLogic = newLogic;
			pumpStation.ChangedLogic();
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
				x.UpdateLogic(DeviceConfiguration);
				x.OnChanged();
			});

			pumpStation.OnChanged();
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

		public static void ChangePumpDevices(GKPumpStation pumpStation, List<GKDevice> devices)
		{
			pumpStation.NSDevices.ForEach(x =>
				{
					if (!devices.Contains(x))
					{
						x.OutputDependentElements.Remove(pumpStation);
						pumpStation.InputDependentElements.Remove(x);
						x.NSLogic = new GKLogic();
						x.OnChanged();
					}
				});

			pumpStation.NSDevices = devices;
			pumpStation.NSDeviceUIDs = new List<Guid>();

			foreach (var device in pumpStation.NSDevices)
			{
				pumpStation.NSDeviceUIDs.Add(device.UID);
				device.Logic = new GKLogic();
				pumpStation.AddDependentElement(device);
			}
			pumpStation.OnChanged();
		}
	}
}