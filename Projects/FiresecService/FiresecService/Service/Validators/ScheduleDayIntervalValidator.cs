using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using FiresecAPI.SKD;
using SKDDriver;
using DayIntervalCommonValidator = EntitiesValidation.DayIntervalValidator;
using ScheduleSchemeCommonValidator = EntitiesValidation.ScheduleSchemeValidator;

namespace FiresecService.Service.Validators
{
	class ScheduleDayIntervalValidator
	{
		/// <summary>
		/// Производит проверку схемы графика работы при добавлении/редактировании дневного графика работы
		/// </summary>
		/// <param name="scheduleDayInterval">Дневной график работы</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public static OperationResult ValidateAddingOrEditing(ScheduleDayInterval scheduleDayInterval)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				// Получаем схему графика работы
				var scheduleSchemeTranslatorGetSingle = databaseService.ScheduleSchemeTranslator.GetSingle(scheduleDayInterval.ScheduleSchemeUID);
				if (scheduleSchemeTranslatorGetSingle.HasError)
					return new OperationResult(scheduleSchemeTranslatorGetSingle.Error);

				var scheduleScheme = scheduleSchemeTranslatorGetSingle.Result;
				
				// Валидируем сменный график
				if (scheduleScheme.Type == ScheduleSchemeType.SlideDay)
				{
					// Получаем дневной график, добавляемый в схему графика работы
					var dayIntervalTranslatorGetSingle = databaseService.DayIntervalTranslator.GetSingle(scheduleDayInterval.DayIntervalUID);
					if (dayIntervalTranslatorGetSingle.HasError)
						return new OperationResult(dayIntervalTranslatorGetSingle.Error);

					// Получаем уже существующие в схеме графика работы дневные графики
					var dayIntervals = databaseService.DayIntervalTranslator.GetDayIntervals(scheduleScheme).ToList();

					// Добавляем новый дневной график или редактируем существующий?
					if (scheduleDayInterval.Number >= dayIntervals.Count)
						dayIntervals.Add(dayIntervalTranslatorGetSingle.Result);
					else
						dayIntervals[scheduleDayInterval.Number] = dayIntervalTranslatorGetSingle.Result;

					// Валидируем соседние дневные графики на факт пересечения интервалов
					return ValidateDayIntervalsIntersecion(dayIntervals);
				}
			}

			// Валидация пройдена
			return new OperationResult();
		}

		/// <summary>
		/// Для сменного графика работы производит проверку на пересечение дневных графиков работы
		/// </summary>
		/// <param name="dayIntervals">Дневные графики работы</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		private static OperationResult ValidateDayIntervalsIntersecion(IList<DayInterval> dayIntervals)
		{
			var result = ScheduleSchemeCommonValidator.ValidateDayIntervalsIntersecion(dayIntervals);

			if (result.HasError)
			{
				return new OperationResult(result.Error);
			}

			// Дневные графики не пересекуются
			return new OperationResult();
		}
	}
}
