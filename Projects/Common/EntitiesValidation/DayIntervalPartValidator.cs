using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StrazhAPI;
using StrazhAPI.SKD;

namespace EntitiesValidation
{
	public static class DayIntervalPartValidator
	{
		private const int _daySeconds = 86400;

		/// <summary>
		/// Проверяем что добавляемый интервал не должен заканчиваться ранее, чем уже добавленные интервалы
		/// </summary>
		/// <param name="newDayIntervalPart">Добавляемый интервал</param>
		/// <param name="existingDayIntervalParts">Ранее добавленные интервалы</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public static OperationResult<bool> ValidateNewDayIntervalPartOrder(DayIntervalPart newDayIntervalPart, IEnumerable<DayIntervalPart> existingDayIntervalParts)
		{
			var endTime = newDayIntervalPart.TransitionType == DayIntervalPartTransitionType.Day
				? newDayIntervalPart.EndTime
				: newDayIntervalPart.EndTime.Add(TimeSpan.FromDays(1));
			var existingEndTimes = existingDayIntervalParts.Select(
				existingDayIntervalPart => existingDayIntervalPart.TransitionType == DayIntervalPartTransitionType.Day
				? existingDayIntervalPart.EndTime
				: existingDayIntervalPart.EndTime.Add(TimeSpan.FromDays(1))).ToList();

			return existingEndTimes.Any(x => x >= endTime)
				? OperationResult<bool>.FromError("Интервалы должны идти последовательно")
				: new OperationResult<bool>(true);
		}

		/// <summary>
		/// Проверяем что текущий интервал не должен пересекаться с остальными интервалами
		/// </summary>
		/// <param name="dayIntervalPart">Текущий интервал</param>
		/// <param name="otherDayIntervalParts">Остальные интервалы</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public static OperationResult<bool> ValidateNewDayIntervalPartIntersection(DayIntervalPart dayIntervalPart, IEnumerable<DayIntervalPart> otherDayIntervalParts)
		{
			return otherDayIntervalParts.Any(x => x.HasIntersectionWith(dayIntervalPart))
				? OperationResult<bool>.FromError("Интервалы не должны пересекаться")
				: new OperationResult<bool>(true);
		}

		/// <summary>
		/// Проверяет что время начала интервала не может равно времени окончания
		/// </summary>
		/// <param name="dayIntervalPart">Интервал</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public static OperationResult<bool> ValidateNewDayIntervalPartLength(DayIntervalPart dayIntervalPart)
		{
			return dayIntervalPart.IsZeroLength()
				? OperationResult<bool>.FromError("Интервал не может иметь нулевую продолжительность")
				: new OperationResult<bool>(true);
		}

		/// <summary>
		/// Проверяет что суммарная продолжительность интервалов дневного графика не может быть меньше значения в поле
		/// "Обязательная продолжительность скользящего графика"
		/// </summary>
		/// <param name="dayIntervalParts">Интервалы дневного графика</param>
		/// <param name="slideTime">Обязательная продолжительность скользящего графика</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public static OperationResult<bool> ValidateGeneralDayIntervalPartsLengthOnEditingOrDeleting(IEnumerable<DayIntervalPart> dayIntervalParts, TimeSpan slideTime)
		{
			// Если "Обязательная продолжительность скользящего графика" = 0, ничего не делаем
			if (slideTime == TimeSpan.Zero)
				return new OperationResult<bool>(true);
			
			var generalLength = TimeSpan.Zero;
			foreach (var dayIntervalPart in dayIntervalParts)
				generalLength = generalLength.Add(GetDayIntervalPartLength(dayIntervalPart));
			return generalLength < slideTime
				? OperationResult<bool>.FromError(String.Format(
					"Суммарная продолжительность интервалов дневного графика ({0}) должна быть больше или равна обязательной продолжительности скользящего графика ({1})",
					String.Format("{0} ч {1} мин", generalLength.Hours, generalLength.Minutes),
					String.Format("{0} ч {1} мин", slideTime.Hours, slideTime.Minutes)))
				: new OperationResult<bool>(true);
		}

		/// <summary>
		/// Вычисляет Продолжительность временного интервала
		/// </summary>
		/// <param name="dayIntervalPart">Временной интервал</param>
		/// <returns>Продолжительность временного интервала</returns>
		private static TimeSpan GetDayIntervalPartLength(DayIntervalPart dayIntervalPart)
		{
			var beginTime = dayIntervalPart.BeginTime.TotalSeconds;
			var endTime = dayIntervalPart.EndTime.TotalSeconds;
			if (dayIntervalPart.TransitionType == DayIntervalPartTransitionType.Night)
				endTime += _daySeconds;
			return TimeSpan.FromSeconds(endTime - beginTime);
		}

		/// <summary>
		/// Проверяет возможность добавления интервала
		/// </summary>
		/// <param name="dayIntervalPart">Добавляемый интервал</param>
		/// <param name="slideTime">Обязательная продолжительность скользящего графика</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public static OperationResult<bool> ValidateDayIntervalPartWithTransitionOnAddingOrEditing(DayIntervalPart dayIntervalPart, TimeSpan slideTime)
		{
			// Если "Обязательная продолжительность скользящего графика" = 0, ничего не делаем
			if (slideTime == TimeSpan.Zero)
				return new OperationResult<bool>(true);

			return dayIntervalPart.TransitionType == DayIntervalPartTransitionType.Night
				? OperationResult<bool>.FromError(
					"Для добавления интервала с переходом необходимо установить в поле \"Суммарная продолжительность интервалов дневного графика\" значение 0")
				: new OperationResult<bool>(true);
		}
	}
}
