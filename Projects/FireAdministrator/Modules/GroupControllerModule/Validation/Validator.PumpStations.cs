using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Validation;
using XFiresecAPI;
using System;

namespace GKModule.Validation
{
	public static partial class Validator
	{
		static void ValidatePumpStations()
		{
			ValidatePumpStationNoEquality();
			ValidateNSDifferentDevices();

			foreach (var pumpStation in XManager.PumpStations)
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

		static void ValidatePumpStationNoEquality()
		{
			var pumpStationNos = new HashSet<int>();
			foreach (var pumpStation in XManager.PumpStations)
			{
				if (!pumpStationNos.Add(pumpStation.No))
					Errors.Add(new PumpStationValidationError(pumpStation, "Дублируется номер", ValidationErrorLevel.CannotWrite));
			}
		}

		static void ValidatePumpStationDifferentDevices()
		{
			var nsDevices = new HashSet<Guid>();
			foreach (var pumpStation in XManager.PumpStations)
			{
				foreach (var device in pumpStation.NSDevices)
				{
					if (!nsDevices.Add(device.UID))
						Errors.Add(new PumpStationValidationError(pumpStation, "Устройство " + device.PresentationName + " участвует в различных НС", ValidationErrorLevel.CannotWrite));
				}
			}
		}

		static void ValidateDifferentGK(XPumpStation pumpStation)
		{
			var devices = new List<XDevice>();
			devices.AddRange(pumpStation.InputDevices);
			devices.AddRange(pumpStation.NSDevices);
			foreach (var zone in pumpStation.InputZones)
			{
				devices.AddRange(zone.Devices);
			}
			foreach (var direction in pumpStation.InputDirections)
			{
				devices.AddRange(direction.InputDevices);
			}

			if (AreDevicesInSameGK(devices))
				Errors.Add(new PumpStationValidationError(pumpStation, "НС содержит объекты устройства разных ГК", ValidationErrorLevel.CannotWrite));
		}

		static bool ValidateEmptyPumpStation(XPumpStation pumpStation)
		{
			var count = pumpStation.InputZones.Count + pumpStation.InputDevices.Count + pumpStation.InputDirections.Count + pumpStation.NSDevices.Count;
			if (count == 0)
			{
				Errors.Add(new PumpStationValidationError(pumpStation, "В НС отсутствуют входные или выходные объекты", ValidationErrorLevel.CannotWrite));
				return false;
			}
			return true;
		}

		static void ValidatePumpStationInput(XPumpStation pumpStation)
		{
			if (pumpStation.InputDevices.Count + pumpStation.InputZones.Count + pumpStation.InputDirections.Count == 0)
				Errors.Add(new PumpStationValidationError(pumpStation, "В НС отсутствуют входные устройства, зоны или направления", ValidationErrorLevel.CannotWrite));
		}

		static void ValidatePumpStationOutput(XPumpStation pumpStation)
		{
			var pumpsCount = pumpStation.NSDevices.Count(x => x.DriverType == XDriverType.Pump && x.IntAddress <= 8);
			if (pumpsCount == 0)
			{
				Errors.Add(new PumpStationValidationError(pumpStation, "В НС отсутствуют насосы", ValidationErrorLevel.CannotWrite));
			}
			else
			{
				if (pumpStation.NSPumpsCount > pumpsCount)
					Errors.Add(new PumpStationValidationError(pumpStation, "В НС основных насосов меньше реально располагаемых", ValidationErrorLevel.CannotWrite));
			}

			var jnPumpsCount = pumpStation.NSDevices.Count(x => x.DriverType == XDriverType.Pump && x.IntAddress == 12);
			if (jnPumpsCount > 1)
				Errors.Add(new PumpStationValidationError(pumpStation, "В НС количество подключенных ЖН больше 1", ValidationErrorLevel.CannotWrite));

			if (jnPumpsCount == 1)
			{
				if (pumpStation.InputZones.Count > 0)
					Errors.Add(new PumpStationValidationError(pumpStation, "В условии пуска НС c ЖН не могут участвовать зоны", ValidationErrorLevel.CannotWrite));

				if (pumpStation.InputDirections.Count > 0)
					Errors.Add(new PumpStationValidationError(pumpStation, "В условии пуска НС c ЖН не могут участвовать направления", ValidationErrorLevel.CannotWrite));

				if (!pumpStation.InputDevices.All(x => x.DriverType == XDriverType.AM_1))
					Errors.Add(new PumpStationValidationError(pumpStation, "В НС c ЖН во входных устройствах могут участвовать только АМ1", ValidationErrorLevel.CannotWrite));
			}
		}

		static void ValidateEmptyObjectsInPumpStation(XPumpStation pumpStation)
		{
			foreach (var zone in pumpStation.InputZones)
			{
				if (zone.Devices.Count == 0)
				{
					Errors.Add(new PumpStationValidationError(pumpStation, "В НС входит пустая зона " + zone.PresentationName, ValidationErrorLevel.CannotWrite));
				}
			}

			foreach (var direction in pumpStation.InputDirections)
			{
				if (direction.InputDevices.Count + direction.InputZones.Where(x=>x.Devices.Count > 0).Count() == 0)
				{
					Errors.Add(new PumpStationValidationError(pumpStation, "В НС входит пустое направление " + direction.PresentationName, ValidationErrorLevel.CannotWrite));
				}
			}
		}

		static void ValidatePumpInNSInputLogic(XPumpStation pumpStation)
		{
			foreach (var clause in pumpStation.StartLogic.Clauses)
			{
				foreach (var device in clause.Devices)
				{
					if (device.DriverType == XDriverType.Pump)
					{
						Errors.Add(new PumpStationValidationError(pumpStation, "В условиях пуска или запрета пуска не может участвовать ШУН", ValidationErrorLevel.CannotWrite));
						return;
					}
				}
			}
		}
	}
}