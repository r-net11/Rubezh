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
				if (IsManyGK())
					ValidateDifferentGK(pumpStation);
				if (ValidateEmptyPumpStation(pumpStation))
				{
					ValidatePumpStationInput(pumpStation);
					ValidatePumpStationOutput(pumpStation);

					ValidateEmptyObjectsInPumpStation(pumpStation);
					ValidatePumpInNSInputLogic(pumpStation);
				}
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
			if (pumpStation.StartLogic.ClausesGroup.Clauses.Count == 0)
				Errors.Add(new PumpStationValidationError(pumpStation, "В НС отсутствует условие для запуска", ValidationErrorLevel.CannotWrite));
		}

		void ValidatePumpStationOutput(GKPumpStation pumpStation)
		{
			var pumpsCount = pumpStation.NSDevices.Count(x => x.Driver.DriverType == GKDriverType.FirePump || x.Driver.DriverType == GKDriverType.RSR2_Bush_Drenazh);
			if (pumpsCount == 0)
			{
				Errors.Add(new PumpStationValidationError(pumpStation, "В НС отсутствуют насосы", ValidationErrorLevel.CannotWrite));
			}
			else
			{
				if (pumpStation.NSPumpsCount > pumpsCount)
					Errors.Add(new PumpStationValidationError(pumpStation, "В НС основных насосов меньше реально располагаемых", ValidationErrorLevel.CannotWrite));
			}

			var jnPumpsCount = pumpStation.NSDevices.Count(x => x.DriverType == GKDriverType.JockeyPump);
			if (jnPumpsCount > 1)
				Errors.Add(new PumpStationValidationError(pumpStation, "В НС количество подключенных ЖН больше 1", ValidationErrorLevel.CannotWrite));

			if (jnPumpsCount == 1)
			{
				if (pumpStation.ClauseInputZones.Count > 0)
					Errors.Add(new PumpStationValidationError(pumpStation, "В условии пуска НС c ЖН не могут участвовать зоны", ValidationErrorLevel.CannotWrite));

				if (pumpStation.ClauseInputDirections.Count > 0)
					Errors.Add(new PumpStationValidationError(pumpStation, "В условии пуска НС c ЖН не могут участвовать направления", ValidationErrorLevel.CannotWrite));

				if (!pumpStation.ClauseInputDevices.All(x => x.DriverType == GKDriverType.AM_1))
					Errors.Add(new PumpStationValidationError(pumpStation, "В НС c ЖН во входных устройствах могут участвовать только АМ1", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateEmptyObjectsInPumpStation(GKPumpStation pumpStation)
		{
			foreach (var zone in pumpStation.ClauseInputZones)
			{
				if (zone.Devices.Count == 0)
				{
					Errors.Add(new PumpStationValidationError(pumpStation, "В НС входит пустая зона " + zone.PresentationName, ValidationErrorLevel.CannotWrite));
				}
			}

			foreach (var direction in pumpStation.ClauseInputDirections)
			{
				if (direction.InputDevices.Count + direction.InputZones.Where(x => x.Devices.Count > 0).Count() == 0)
				{
					Errors.Add(new PumpStationValidationError(pumpStation, "В НС входит пустое направление " + direction.PresentationName, ValidationErrorLevel.CannotWrite));
				}
			}
		}

		void ValidatePumpInNSInputLogic(GKPumpStation pumpStation)
		{
			foreach (var clause in pumpStation.StartLogic.ClausesGroup.Clauses)
			{
				foreach (var device in clause.Devices)
				{
					if (device.Driver.IsPump)
					{
						Errors.Add(new PumpStationValidationError(pumpStation, "В условии для запуска не может участвовать ШУН", ValidationErrorLevel.CannotWrite));
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
						Errors.Add(new DeviceValidationError(nsDevice, "Устройство присутствует в разных НС (" + pumpStation.PresentationName + ")", ValidationErrorLevel.CannotWrite));
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