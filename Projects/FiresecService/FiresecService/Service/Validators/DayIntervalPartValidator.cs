using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntitiesValidation;
using StrazhAPI;
using StrazhAPI.SKD;
using StrazhDAL;
using DayInterval = StrazhAPI.SKD.DayInterval;
using DayIntervalPart = StrazhAPI.SKD.DayIntervalPart;
using DayIntervalPartCommonValidator = EntitiesValidation.DayIntervalPartValidator;
using ScheduleScheme = StrazhAPI.SKD.ScheduleScheme;

namespace FiresecService.Service.Validators
{
	public static class DayIntervalPartValidator
	{
		/// <summary>
		/// Проводит валидацию при добавлении/редактировании временного интервала в дневном графике
		/// </summary>
		/// <param name="dayIntervalPart">Добавляемый/отредактированный временной интервал</param>
		/// <param name="isNew">Признак что временной интервал добавляется (новый)</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public static OperationResult ValidateAddingOrEditing(DayIntervalPart dayIntervalPart, bool isNew)
		{
			return isNew ? ValidateAdding(dayIntervalPart) : ValidateEditing(dayIntervalPart);
		}

		/// <summary>
		/// Проводит валидацию при добавлении временного интервала в дневной график
		/// </summary>
		/// <param name="dayIntervalPart">Добавляемый временной интервал</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		private static OperationResult ValidateAdding(DayIntervalPart dayIntervalPart)
		{
			// Базовая валидация
			var validationResult = ValidateAddingCommon(dayIntervalPart);
			if (validationResult.HasError)
				return validationResult;

			// Валидация по связям
			validationResult = ValidateAddingComplex(dayIntervalPart);
			if (validationResult.HasError)
				return validationResult;

			// Валидация сменного графика работы на пересечение дневных графиков
			validationResult = ValidateDayIntervalsIntersectionOnAdding(dayIntervalPart);

			return validationResult;
		}

		/// <summary>
		/// Проводит валидацию при редактировании временного интервала в дневном графике
		/// </summary>
		/// <param name="dayIntervalPart">Отредактированный временной интервал</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		private static OperationResult ValidateEditing(DayIntervalPart dayIntervalPart)
		{
			// Базовая валидация
			var validationResult = ValidateEditingCommon(dayIntervalPart);
			if (validationResult.HasError)
				return validationResult;

			// Валидация по связям
			validationResult = ValidateEditingComplex(dayIntervalPart);
			if (validationResult.HasError)
				return validationResult;

			// Валидация сменного графика работы на пересечение дневных графиков
			validationResult = ValidateDayIntervalsIntersectionOnEditing(dayIntervalPart);
			return validationResult;
		}

		/// <summary>
		/// Проводит базовую валидацию временного интервала дневного графика
		/// </summary>
		/// <param name="dayIntervalPart">Временной интервал</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		private static OperationResult ValidateCommon(DayIntervalPart dayIntervalPart)
		{
			// Время начала интервала равно времени окончания?
			var validationResult = DayIntervalPartCommonValidator.ValidateNewDayIntervalPartLength(dayIntervalPart);
			if (validationResult.HasError)
				return new OperationResult(validationResult.Errors);

			IEnumerable<DayIntervalPart> otherDayIntervalParts;
			using (var databaseService = new SKDDatabaseService())
			{
				otherDayIntervalParts = databaseService.DayIntervalPartTranslator.GetOtherDayIntervalParts(dayIntervalPart).ToList();
			}
			
			// Если другие временные интервалы отсутствуют, то дальше не выполняем проверку
			if (otherDayIntervalParts.Any())
			{
				// Временной интервал пересекается с остальными интервалами?
				validationResult = DayIntervalPartCommonValidator.ValidateNewDayIntervalPartIntersection(dayIntervalPart, otherDayIntervalParts);
				if (validationResult.HasError)
					return new OperationResult(validationResult.Errors);
			}

			return new OperationResult();
		}

		private static OperationResult ValidateDayIntervalPartWithTransitionOnAddingOrEditing(DayIntervalPart dayIntervalPart)
		{
			DayInterval dayInterval = null;
			using (var databaseService = new SKDDatabaseService())
			{
				dayInterval = databaseService.DayIntervalTranslator.GetDayInterval(dayIntervalPart);
			}
			if (dayInterval == null)
                return new OperationResult(Resources.Language.Service.Validators.DayIntervalPartValidator.ValidateDayIntervalPartWithTransitionOnAddingOrEditing_Error);

			var operationResult = DayIntervalPartCommonValidator.ValidateDayIntervalPartWithTransitionOnAddingOrEditing(dayIntervalPart, dayInterval.SlideTime);
			if (operationResult.HasError)
				return new OperationResult(operationResult.Errors);
			return new OperationResult();
		}

		private static OperationResult ValidateAddingCommon(DayIntervalPart dayIntervalPart)
		{
			// Базовая валидация
			var validationResult = ValidateCommon(dayIntervalPart);
			if (validationResult.HasError)
				return validationResult;

			IEnumerable<DayIntervalPart> otherDayIntervalParts;
			using (var databaseService = new SKDDatabaseService())
			{
				otherDayIntervalParts = databaseService.DayIntervalPartTranslator.GetOtherDayIntervalParts(dayIntervalPart).ToList();
			}

			if (otherDayIntervalParts.Any())
			{
				// Добавляемый временной интервал заканчивается ранее, чем остальные интервалы?
				var validationesult = DayIntervalPartCommonValidator.ValidateNewDayIntervalPartOrder(dayIntervalPart, otherDayIntervalParts);
				if (validationesult.HasError)
					return new OperationResult(validationesult.Errors);
			}

			// Если значение в поле "Обязательная продолжительность скользящего графика" > 0, то добавить интервал с переходом на следующие сутки нельзя
			return ValidateDayIntervalPartWithTransitionOnAddingOrEditing(dayIntervalPart);
		}

		private static OperationResult ValidateAddingComplex(DayIntervalPart dayIntervalPart)
		{
            var error = String.Format(Resources.Language.Service.Validators.DayIntervalPartValidator.ValidateAddingComplex_Error);

			DayInterval dayInterval;
			IEnumerable<ScheduleScheme> scheduleSchemes;
			using (var databaseService = new SKDDatabaseService())
			{
				dayInterval = databaseService.DayIntervalTranslator.GetDayInterval(dayIntervalPart);
				if (dayInterval == null)
					return new OperationResult(error);

				scheduleSchemes = databaseService.ScheduleSchemeTranslator.GetScheduleSchemes(dayInterval);
			}

			// Если дневной график не входит в состав какого-либо графика, выходим
			if (!scheduleSchemes.Any())
				return new OperationResult();

			// Если "Обязательная продолжительность скользящего графика" = 0
			if (dayInterval.SlideTime == TimeSpan.Zero)
			{
				var weekOrMonthScheduleSchemes = scheduleSchemes.Where(x => x.Type == ScheduleSchemeType.Week || x.Type == ScheduleSchemeType.Month);
				// Если тип связного графика - не "Недельный" и не "Месячный", выходим
				if (!weekOrMonthScheduleSchemes.Any())
					return new OperationResult();
				// Если добавляемый интервал не является переходным, выходим
				if (dayIntervalPart.TransitionType != DayIntervalPartTransitionType.Night)
					return new OperationResult();
				var weekOrMonthScheduleSchemesStr = new StringBuilder();
				foreach (var weekOrMonthScheduleScheme in weekOrMonthScheduleSchemes)
				{
					weekOrMonthScheduleSchemesStr.AppendLine(String.Format("{0} ({1}{2})", weekOrMonthScheduleScheme.Name, weekOrMonthScheduleScheme.Type.ToDescription().ToLower(), weekOrMonthScheduleScheme.IsDeleted ? ", архивный" : null));
				}
					return new OperationResult(String.Format(
                        Resources.Language.Service.Validators.DayIntervalPartValidator.ValidateAddingComplex,
						weekOrMonthScheduleSchemesStr));
			}
			return new OperationResult();
		}

		private static OperationResult ValidateDayIntervalsIntersectionOnAdding(DayIntervalPart dayIntervalPart)
		{
			var sb = new StringBuilder();

			using (var databaseService = new SKDDatabaseService())
			{
				// Для интервала дневного графика получаем связанные схемы графика работы с типом "Сменный"
				var scheduleSchemes = databaseService.ScheduleSchemeTranslator.GetScheduleSchemes(dayIntervalPart).Where(x => x.Type == ScheduleSchemeType.SlideDay);
				
				// Проводим валидацию каждой найденной схемы графика работы
				foreach (var scheduleScheme in scheduleSchemes)
				{
					// Получаем дневные графики для схемы графика работы
					var dayIntervals = databaseService.DayIntervalTranslator.GetDayIntervals(scheduleScheme).ToList();

					// В дневной график добавляем новый временной интервал
					var editedDayInterval = dayIntervals.FirstOrDefault(y => y.UID == dayIntervalPart.DayIntervalUID);
					editedDayInterval.DayIntervalParts.Add(dayIntervalPart);

					// Проводим валидацию дневных интервалов текущей схемы графика работы на пересечение
					var validationResult = ScheduleSchemeValidator.ValidateDayIntervalsIntersecion(dayIntervals);
					if (validationResult.HasError)
					{
                        sb.AppendLine(String.Format(Resources.Language.Service.Validators.DayIntervalPartValidator.ValidateDayIntervalsIntersectionOnAdding, scheduleScheme.Name));
						sb.AppendLine(validationResult.Error);
					}
				}
			}

			if (sb.Length > 0)
				return new OperationResult(sb.ToString());

			// Дневные графики не пересекаются
			return new OperationResult();
		}

		private static OperationResult ValidateEditingCommon(DayIntervalPart dayIntervalPart)
		{
			// Базовая валидация
			var validationResult = ValidateCommon(dayIntervalPart);
			if (validationResult.HasError)
				return validationResult;

			// Если значение в поле "Обязательная продолжительность скользящего графика" > 0, то добавить интервал с переходом на следующие сутки нельзя
			var dayIntervalPartWithTransitionValidationResult = ValidateDayIntervalPartWithTransitionOnAddingOrEditing(dayIntervalPart);
			if (dayIntervalPartWithTransitionValidationResult.HasError)
				return dayIntervalPartWithTransitionValidationResult;
			
			// Суммарная продолжительность интервалов дневного графика меньше значения в поле "Обязательная продолжительность скользящего графика"?
			DayInterval dayInterval;
			IEnumerable<DayIntervalPart> otherDayIntervalParts;
			using (var databaseService = new SKDDatabaseService())
			{
				dayInterval = databaseService.DayIntervalTranslator.GetDayInterval(dayIntervalPart);
				otherDayIntervalParts = databaseService.DayIntervalPartTranslator.GetOtherDayIntervalParts(dayIntervalPart);
			}
			var dayIntervalParts = new List<DayIntervalPart>();
			dayIntervalParts.Add(dayIntervalPart);
			dayIntervalParts.AddRange(otherDayIntervalParts);
			var lengthValidationResult = DayIntervalPartCommonValidator.ValidateGeneralDayIntervalPartsLengthOnEditingOrDeleting(dayIntervalParts, dayInterval.SlideTime);
			if (lengthValidationResult.HasError)
				return new OperationResult(lengthValidationResult.Errors);
			return new OperationResult();
		}

		private static OperationResult ValidateEditingComplex(DayIntervalPart dayIntervalPart)
		{
            var error = String.Format(Resources.Language.Service.Validators.DayIntervalPartValidator.ValidateEditingComplex);

			using (var databaseService = new SKDDatabaseService())
			{
				var dayInterval = databaseService.DayIntervalTranslator.GetDayInterval(dayIntervalPart);
				if (dayInterval == null)
					return new OperationResult(error);

				var scheduleSchemes = databaseService.ScheduleSchemeTranslator.GetScheduleSchemes(dayInterval);

				// Если дневной график не входит в состав какого-либо графика, выходим
				if (!scheduleSchemes.Any())
					return new OperationResult();

				// Если "Обязательная продолжительность скользящего графика" = 0
				if (dayInterval.SlideTime == TimeSpan.Zero)
				{
					var weekOrMonthScheduleSchemes = scheduleSchemes.Where(x => x.Type == ScheduleSchemeType.Week || x.Type == ScheduleSchemeType.Month);
					// Если тип связного графика - не "Недельный" и не "Месячный", выходим
					if (!weekOrMonthScheduleSchemes.Any())
						return new OperationResult();
					// Если редактируемый интервал не является переходным, выходим
					if (dayIntervalPart.TransitionType != DayIntervalPartTransitionType.Night)
						return new OperationResult();
					var weekOrMonthScheduleSchemesStr = new StringBuilder();
					foreach (var weekOrMonthScheduleScheme in weekOrMonthScheduleSchemes)
					{
						weekOrMonthScheduleSchemesStr.AppendLine(String.Format("{0} ({1}{2})", weekOrMonthScheduleScheme.Name,
							weekOrMonthScheduleScheme.Type.ToDescription().ToLower(),
                            weekOrMonthScheduleScheme.IsDeleted ? Resources.Language.Service.Validators.DayIntervalPartValidator.ValidateEditingComplex_archive : null));
					}
					return new OperationResult(String.Format(
                        Resources.Language.Service.Validators.DayIntervalPartValidator.ValidateAddingComplex,
						weekOrMonthScheduleSchemesStr));
				}

				return new OperationResult();
			}
		}

		private static OperationResult ValidateDayIntervalsIntersectionOnEditing(DayIntervalPart dayIntervalPart)
		{
			var sb = new StringBuilder();

			using (var databaseService = new SKDDatabaseService())
			{
				// Для интервала дневного графика получаем связанные схемы графика работы с типом "Сменный"
				var scheduleSchemes = databaseService.ScheduleSchemeTranslator.GetScheduleSchemes(dayIntervalPart).Where(x => x.Type == ScheduleSchemeType.SlideDay);

				// Проводим валидацию каждой найденной схемы графика работы
				foreach (var scheduleScheme in scheduleSchemes)
				{
					// Получаем дневные графики для схемы графика работы
					var dayIntervals = databaseService.DayIntervalTranslator.GetDayIntervals(scheduleScheme).ToList();

					// В модифицированном дневном графике заменяем старый временной интервал на новый
					var editedDayInterval = dayIntervals.FirstOrDefault(y => y.UID == dayIntervalPart.DayIntervalUID);
					var index = editedDayInterval.DayIntervalParts.IndexOf(editedDayInterval.DayIntervalParts.FirstOrDefault(x => x.UID == dayIntervalPart.UID));
					editedDayInterval.DayIntervalParts[index] = dayIntervalPart;

					// Проводим валидацию дневных интервалов текущей схемы графика работы на пересечение
					var validationResult = ScheduleSchemeValidator.ValidateDayIntervalsIntersecion(dayIntervals);
					if (validationResult.HasError)
					{
                        sb.AppendLine(String.Format(Resources.Language.Service.Validators.DayIntervalPartValidator.ValidateDayIntervalsIntersectionOnAdding, scheduleScheme.Name));
						sb.AppendLine(validationResult.Error);
					}
				}
			}

			if (sb.Length > 0)
				return new OperationResult(sb.ToString());

			// Дневные графики не пересекаются
			return new OperationResult();
		}

		/// <summary>
		/// Проводит валидацию при удалении временного интервала из дневного графика
		/// </summary>
		/// <param name="dayIntervalPartUID">Идентификатор удаляемого временного интервала</param>
		/// <returns></returns>
		public static OperationResult ValidateDeleting(Guid dayIntervalPartUID)
		{
            var error = String.Format(Resources.Language.Service.Validators.DayIntervalPartValidator.ValidateDeleting, dayIntervalPartUID);
			OperationResult<DayInterval> dayIntervalOperationResult;
			DayInterval dayInterval;
			using (var databaseService = new SKDDatabaseService())
			{
				var dayIntervalPartOperationResult = databaseService.DayIntervalPartTranslator.GetSingle(dayIntervalPartUID);
				if (dayIntervalPartOperationResult.HasError)
					return new OperationResult(dayIntervalPartOperationResult.Errors);

				var dayIntervalPart = dayIntervalPartOperationResult.Result;
				if (dayIntervalPart == null)
					return new OperationResult(error);
				dayInterval = databaseService.DayIntervalTranslator.GetDayInterval(dayIntervalPart);
			}
			if (dayInterval == null)
				return new OperationResult(error);

			var validationResult =
				DayIntervalPartCommonValidator.ValidateGeneralDayIntervalPartsLengthOnEditingOrDeleting(
					dayInterval.DayIntervalParts.Where(x => x.UID != dayIntervalPartUID), dayInterval.SlideTime);
			if (validationResult.HasError)
				return new OperationResult(validationResult.Errors);
			
			return new OperationResult();
		}
	}
}
