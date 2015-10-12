using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
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
			if (pumpStation.GkParents.Count > 1)
				Errors.Add(new PumpStationValidationError(pumpStation, "НС содержит объекты разных ГК", ValidationErrorLevel.CannotWrite));
		}

		bool ValidateEmptyPumpStation(GKPumpStation pumpStation)
		{
			if (!pumpStation.InputDependentElements.Any() || !pumpStation.NSDevices.Any())
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