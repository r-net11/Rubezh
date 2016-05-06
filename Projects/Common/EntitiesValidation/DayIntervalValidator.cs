using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StrazhAPI;
using StrazhAPI.SKD;

namespace EntitiesValidation
{
	public static class DayIntervalValidator
	{
		/// <summary>
		/// Проверяет пересекаются или нет интервалы смежных дневных графиков
		/// </summary>
		/// <param name="dayInterval1">Дневной график 1, который может содержать интервал с переходом, пересекающийся с одним или более интарвалами Дневного графика 2</param>
		/// <param name="dayInterval2">Дневной график 2</param>
		/// <returns>Объект OperationResult с результатом выполнения операции
		/// Содержит список интервалов Дневного графика 2, с которыми пересекается интервал с переходом из Дневного графика 1</returns>
		public static OperationResult<List<DayIntervalPart>> ValidateIntersection(DayInterval dayInterval1, DayInterval dayInterval2)
		{
			// Согласно бизнесу может быть только один интервал с переходом
			var dayInterval1LastDayIntervalPart = dayInterval1.DayIntervalParts.FirstOrDefault(x => x.TransitionType == DayIntervalPartTransitionType.Night);

			if (dayInterval1LastDayIntervalPart == null || dayInterval1LastDayIntervalPart.TransitionType != DayIntervalPartTransitionType.Night)
				// Предыдущий день не содержит временной интервал с переходом - пересечения не будет
				return new OperationResult<List<DayIntervalPart>>(new List<DayIntervalPart>());

			// Находим часть интевала dayInterval1, переходящую на следующий день dayInterval2
			var dayIntervalPartInNextDay = dayInterval1LastDayIntervalPart.GetDayIntervalPartInNextDayInterval();
			
			// Если есть пересечение найденной части интервала с интервалами в dayInterval2, возвращаем ошибку валидации
			var dayIntervalPartsWithIntersection = dayInterval2.DayIntervalParts.Where(x => x.HasIntersectionWith(dayIntervalPartInNextDay)).ToList();
			if (dayIntervalPartsWithIntersection.Count > 0)
			{
				var sb = new StringBuilder();
				sb.AppendLine(String.Format("Дневной график '{0}' пересекает временные интервалы дневного графика '{1}':", dayInterval1.Name, dayInterval2.Name));
				foreach (var dayIntervalPart in dayIntervalPartsWithIntersection)
				{
					sb.AppendLine(String.Format("- [{0}-{1}]", dayIntervalPart.BeginTime.ToString(@"hh\:mm\:ss"), dayIntervalPart.EndTime.ToString(@"hh\:mm\:ss")));
				}
				return OperationResult<List<DayIntervalPart>>.FromError(sb.ToString(), dayIntervalPartsWithIntersection);
			}

			// Пересечения не выявлены
			return new OperationResult<List<DayIntervalPart>>(new List<DayIntervalPart>());
		}
	}
}
