using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StrazhAPI;
using StrazhAPI.SKD;
using StrazhDAL;
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

					var addingOrEditingDayInterval = dayIntervalTranslatorGetSingle.Result ?? DayIntervalCreator.CreateDayIntervalNever();

					// Получаем уже существующие в схеме графика работы дневные графики
					var dayIntervals = databaseService.DayIntervalTranslator.GetDayIntervals(scheduleScheme).ToList();

					// Добавляем новый дневной график или редактируем существующий?
					if (scheduleDayInterval.Number >= dayIntervals.Count)
						dayIntervals.Add(addingOrEditingDayInterval);
					else
						dayIntervals[scheduleDayInterval.Number] = addingOrEditingDayInterval;

					// Валидируем соседние дневные графики на факт пересечения интервалов
					return ValidateDayIntervalsIntersecion(dayIntervals);
				}
			}

			// Валидация пройдена
			return new OperationResult();
		}

		public static OperationResult ValidateDeleting(Guid scheduleDayIntervalUid)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				// Получаем объект ScheduleDayInterval на который ссылается scheduleDayIntervalUid
				var scheduleDayIntervalGetSingle = databaseService.ScheduleDayIntervalTranslator.GetSingle(scheduleDayIntervalUid);
				if (scheduleDayIntervalGetSingle.HasError)
					return new OperationResult(scheduleDayIntervalGetSingle.Error);

				var scheduleDayInterval = scheduleDayIntervalGetSingle.Result;

				// Получаем схему графика работы
				var scheduleSchemeTranslatorGetSingle = databaseService.ScheduleSchemeTranslator.GetSingle(scheduleDayInterval.ScheduleSchemeUID);
				if (scheduleSchemeTranslatorGetSingle.HasError)
					return new OperationResult(scheduleSchemeTranslatorGetSingle.Error);

				var scheduleScheme = scheduleSchemeTranslatorGetSingle.Result;

				// Валидируем сменный график
				if (scheduleScheme.Type == ScheduleSchemeType.SlideDay)
				{
					// Получаем уже существующие в схеме графика работы дневные графики
					var dayIntervals = databaseService.DayIntervalTranslator.GetDayIntervals(scheduleScheme).ToList();

					// Удаляем из них удаляемый дневной график
					dayIntervals.RemoveAt(scheduleDayInterval.Number);

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
