using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using FiresecAPI.SKD;

namespace EntitiesValidation
{
	public static class ScheduleSchemeValidator
	{
		/// <summary>
		/// Для сменного графика работы производит проверку на пересечение между дневными графиками работы
		/// </summary>
		/// <param name="dayIntervals">Дневные графики работы</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public static OperationResult<List<DayInterval>> ValidateDayIntervalsIntersecion(IList<DayInterval> dayIntervals)
		{
			var sb = new StringBuilder();
			var dayIntervalsWithIntersection = new List<DayInterval>();
			for (var i = 0; i < dayIntervals.Count; i++)
			{
				var j = i < (dayIntervals.Count - 1) ? i + 1 : 0;
				var dayIntervalCommonValidatorValidateIntersection = DayIntervalValidator.ValidateIntersection(dayIntervals[i], dayIntervals[j]);
				if (dayIntervalCommonValidatorValidateIntersection.HasError)
				{
					sb.AppendLine(dayIntervalCommonValidatorValidateIntersection.Error);
					dayIntervalsWithIntersection.Add(dayIntervals[j]);
				}
			}

			if (dayIntervalsWithIntersection.Count > 0)
			{
				return OperationResult<List<DayInterval>>.FromError(sb.ToString(), dayIntervalsWithIntersection);
			}

			// Дневные графики не пересекуются
			return new OperationResult<List<DayInterval>>();
		}
	}
}
