using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Validation;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidatePumpStations()
		{
			ValidatePumpStationNoEquality();
			ValidatePumpsInDifferentNS();

			foreach (var pumpStation in GKManager.PumpStations)
			{
				if (IsManyGK)
					ValidateDifferentGK(pumpStation);
				if (ValidateEmptyPumpStation(pumpStation))
				{
					ValidateEmpty(pumpStation);
					ValidatePumpStationInput(pumpStation);
					ValidatePumpStationOutput(pumpStation);

					ValidateEmptyObjectsInPumpStation(pumpStation);
					ValidatePumpInNSInputLogic(pumpStation);
				}
			}
		}

		void ValidateEmpty(GKPumpStation pumpStation)
		{
			if (pumpStation.DataBaseParent == null)
			{
				Errors.Add(new PumpStationValidationError(pumpStation, "Пустые зависимости", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidatePumpStationNoEquality()
		{
			var pumpStationNos = new HashSet<int>();
			foreach (var pumpStation in GKManager.PumpStations)
			{
				if (!pumpStationNos.Add(pumpStation.No))
					Errors.Add(new PumpStationValidationError(pumpStation, "Дублируется номер", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidatePumpStationDifferentDevices()
		{
			var nsDevices = new HashSet<Guid>();
			foreach (var pumpStation in GKManager.PumpStations)
			{
				foreach (var device in pumpStation.NSDevices)
				{
					if (!nsDevices.Add(device.UID))
						Errors.Add(new PumpStationValidationError(pumpStation, "Устройство " + device.PresentationName + " участвует в различных НС", ValidationErrorLevel.CannotWrite));
				}
			}
		}

		void ValidateDifferentGK(GKPumpStation pumpStation)
		{
			var devices = new List<GKDevice>();
			devices.AddRange(pumpStation.ClauseInputDevices);
			devices.AddRange(pumpStation.NSDevices);
			foreach (var zone in pumpStation.ClauseInputZones)
			{
				devices.AddRange(zone.Devices);
			}
			foreach (var direction in pumpStation.ClauseInputDirections)
			{
				devices.AddRange(direction.InputDevices);
			}

			if (AreDevicesInSameGK(devices))
				Errors.Add(new PumpStationValidationError(pumpStation, "НС содержит объекты устройства разных ГК", ValidationErrorLevel.CannotWrite));
		}

		bool ValidateEmptyPumpStation(GKPumpStation pumpStation)
		{
			var count = pumpStation.ClauseInputZones.Count + pumpStation.ClauseInputDevices.Count + pumpStation.ClauseInputDirections.Count + pumpStation.NSDevices.Count;
			if (count == 0)
			{
				Errors.Add(new PumpStationValidationError(pumpStation, "В НС отсутствуют входные или выходные объекты", ValidationErrorLevel.CannotWrite));
				return false;
			}
			return true;
		}

		void ValidatePumpStationInput(GKPumpStation pumpStation)
		{
			if (pumpStation.StartLogic.OnClausesGroup.Clauses.Count == 0)
				Errors.Add(new PumpStationValidationError(pumpStation, "В НС отсутствует условие для запуска", ValidationErrorLevel.CannotWrite));
		}

		void ValidatePumpStationOutput(GKPumpStation pumpStation)
		{
			var pumpsCount = pumpStation.NSDevices.Count(x => x.Driver.DriverType == GKDriverType.RSR2_Bush_Drenazh || x.Driver.DriverType == GKDriverType.RSR2_Bush_Jokey || x.Driver.DriverType == GKDriverType.RSR2_Bush_Fire || x.Driver.DriverType == GKDriverType.RSR2_Bush_Shuv);
			if (pumpsCount == 0)
			{
				Errors.Add(new PumpStationValidationError(pumpStation, "В НС отсутствуют насосы", ValidationErrorLevel.CannotWrite));
			}
			else
			{
				if (pumpStation.NSPumpsCount > pumpsCount)
					Errors.Add(new PumpStationValidationError(pumpStation, "В НС основных насосов меньше реально располагаемых", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateEmptyObjectsInPumpStation(GKPumpStation pumpStation)
		{
			foreach (var zone in pumpStation.ClauseInputZones)
			{
				if (zone.DataBaseParent == null)
				{
					Errors.Add(new PumpStationValidationError(pumpStation, "В НС входит пустая зона " + zone.PresentationName, ValidationErrorLevel.CannotWrite));
				}
			}

			foreach (var direction in pumpStation.ClauseInputDirections)
			{
				if (direction.DataBaseParent == null)
				{
					Errors.Add(new PumpStationValidationError(pumpStation, "В НС входит пустое направление " + direction.PresentationName, ValidationErrorLevel.CannotWrite));
				}
			}
		}

		void ValidatePumpInNSInputLogic(GKPumpStation pumpStation)
		{
			foreach (var clause in pumpStation.StartLogic.OnClausesGroup.Clauses)
			{
				foreach (var device in clause.Devices)
				{
					if (device.Driver.IsPump)
					{
						Errors.Add(new PumpStationValidationError(pumpStation, "В условии для запуска не может участвовать насос", ValidationErrorLevel.CannotWrite));
						return;
					}
				}
			}
		}

		void ValidatePumpsInDifferentNS()
		{
			var nsDevices = new List<GKDevice>();
			foreach (var pumpStation in GKManager.PumpStations)
			{
				foreach (var nsDevice in pumpStation.NSDevices)
				{
					if (nsDevices.Contains(nsDevice))
					{
						Errors.Add(new DeviceValidationError(nsDevice, "Устройство присутствует в разных НС (" + pumpStation.PresentationName + ")", ValidationErrorLevel.Warning));
					}
					else
					{
						nsDevices.Add(nsDevice);
					}
				}
			}
		}
	}
}