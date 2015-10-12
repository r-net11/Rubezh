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
				if (ValidateObjectsOnlyOnOneGK(pumpStation))
				{
					ValidatePumpStationHasValidDriverTypes(pumpStation);
					ValidatePumpStationEmptyStartLogic(pumpStation);
					ValidatePumpStationPumpsCount(pumpStation);
				}
			}
		}

		/// <summary>
		/// Валидация наличия НС с одинаковыми номерами
		/// </summary>
		void ValidatePumpStationNoEquality()
		{
			var pumpStationNos = new HashSet<int>();
			foreach (var pumpStation in GKManager.PumpStations)
			{
				if (!pumpStationNos.Add(pumpStation.No))
					Errors.Add(new PumpStationValidationError(pumpStation, "Дублируется номер", ValidationErrorLevel.CannotWrite));
			}
		}

		/// <summary>
		/// Если насос учавствует в одной НС, то он не может учавствовать в другой НС
		/// </summary>
		void ValidatePumpsInDifferentNS()
		{
			var devices = new HashSet<Guid>();
			foreach (var pumpStation in GKManager.PumpStations)
			{
				foreach (var device in pumpStation.NSDevices)
				{
					if (!devices.Add(device.UID))
						Errors.Add(new DeviceValidationError(device, "Устройство присутствует в разных НС (" + pumpStation.PresentationName + ")", ValidationErrorLevel.CannotWrite));
				}
			}
		}

		/// <summary>
		/// НС должна содержать объекты, присутствующие на одном и только на одном ГК
		/// </summary>
		/// <param name="pumpStation"></param>
		/// <returns></returns>
		bool ValidateObjectsOnlyOnOneGK(GKPumpStation pumpStation)
		{
			if (pumpStation.GkParents.Count == 0)
			{
				Errors.Add(new PumpStationValidationError(pumpStation, "Пустые зависимости", ValidationErrorLevel.CannotWrite));
				return false;
			}

			if (pumpStation.GkParents.Count > 1)
			{
				Errors.Add(new PumpStationValidationError(pumpStation, "НС содержит объекты разных ГК", ValidationErrorLevel.CannotWrite));
				return false;
			}
			return true;
		}

		/// <summary>
		/// НС должно содержать в списке устройств только насосы
		/// Конфигурация этого не дает сделать, но на всякий случай
		/// </summary>
		/// <param name="pumpStation"></param>
		void ValidatePumpStationHasValidDriverTypes(GKPumpStation pumpStation)
		{
			if (pumpStation.NSDevices.Any(x => !x.Driver.IsPump))
			{
				Errors.Add(new PumpStationValidationError(pumpStation, "В НС отсутствуют насосы", ValidationErrorLevel.CannotWrite));
			}
		}

		/// <summary>
		/// Условие запуска НС должно иметь объекты
		/// </summary>
		/// <param name="pumpStation"></param>
		void ValidatePumpStationEmptyStartLogic(GKPumpStation pumpStation)
		{
			if (pumpStation.StartLogic.GetObjects().Count == 0)
				Errors.Add(new PumpStationValidationError(pumpStation, "В НС отсутствует условие для запуска", ValidationErrorLevel.CannotWrite));
		}

		/// <summary>
		/// Количество устройств, входящих в НС, должно быть больше количества основных насосов
		/// Таким же образом проверяется отсутствие насосов вообще
		/// </summary>
		/// <param name="pumpStation"></param>
		void ValidatePumpStationPumpsCount(GKPumpStation pumpStation)
		{
			if (pumpStation.NSPumpsCount > pumpStation.NSDevices.Count)
				Errors.Add(new PumpStationValidationError(pumpStation, "В НС основных насосов меньше реально располагаемых", ValidationErrorLevel.CannotWrite));
		}
	}
}