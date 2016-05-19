using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Validation;
using RubezhAPI;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidatePumpStations()
		{
			ValidateCommon(GKManager.PumpStations);
			ValidatePumpsInDifferentNS();

			foreach (var pumpStation in GKManager.PumpStations)
			{
				ValidatePumpStationHasValidDriverTypes(pumpStation);
				ValidatePumpStationEmptyStartLogic(pumpStation);
				ValidatePumpStationPumpsCount(pumpStation);
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
						AddError(device, "Устройство присутствует в разных НС (" + pumpStation.PresentationName + ")", ValidationErrorLevel.CannotWrite);
				}
			}
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
				AddError(pumpStation, "В НС отсутствуют насосы", ValidationErrorLevel.CannotWrite);
			}
		}

		/// <summary>
		/// Условие запуска НС должно иметь объекты
		/// </summary>
		/// <param name="pumpStation"></param>
		void ValidatePumpStationEmptyStartLogic(GKPumpStation pumpStation)
		{
			if (pumpStation.StartLogic.GetObjects().Count == 0)
				AddError(pumpStation, "В НС отсутствует условие для запуска", ValidationErrorLevel.CannotWrite);
		}

		/// <summary>
		/// Количество устройств, входящих в НС, должно быть больше количества основных насосов
		/// Таким же образом проверяется отсутствие насосов вообще
		/// </summary>
		/// <param name="pumpStation"></param>
		void ValidatePumpStationPumpsCount(GKPumpStation pumpStation)
		{
			if (pumpStation.NSPumpsCount > pumpStation.NSDevices.Count)
				AddError(pumpStation, "В НС основных насосов меньше реально располагаемых", ValidationErrorLevel.CannotWrite);
		}
	}
}