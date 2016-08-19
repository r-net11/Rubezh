using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using StrazhAPI;
using StrazhAPI.SKD;
using StrazhDAL;

namespace FiresecService.Service.Validators
{
	public static class DayIntervalValidator
	{
		public static OperationResult ValidateAddingOrEditing(DayInterval dayInterval, bool isNew)
		{
			return isNew ? ValidateAdding(dayInterval) : ValidateEditing(dayInterval);
		}

		private static OperationResult ValidateAdding(DayInterval dayInterval)
		{
			// Для нового дневного графика валидация не требуется
			return new OperationResult();
		}

		private static OperationResult ValidateEditing(DayInterval dayInterval)
		{
			if (dayInterval.SlideTime == TimeSpan.Zero)
				return new OperationResult();

			return ValidateScheduleSchemes(dayInterval);
		}

		private static OperationResult<IEnumerable<ScheduleScheme>> GetScheduleSchemes(Guid dayIntervalUID)
		{
			OperationResult<IEnumerable<ScheduleScheme>> result;
			using (var databaseService = new SKDDatabaseService())
			{
				result =
					databaseService.ScheduleSchemeTranslator.Get(new ScheduleSchemeFilter
					{
						DayIntervalUIDs = new List<Guid> { dayIntervalUID }
					});
			}
			return result;
		}

		private static OperationResult ValidateScheduleSchemes(DayInterval dayInterval)
		{
			// Проявляем принадлежнось дневного графика к месячному или сменном графику?
			var operationResult = GetScheduleSchemes(dayInterval.UID);

			if (operationResult.HasError)
				return new OperationResult(operationResult.Errors);

			var scheduleSchemes = operationResult.Result;

			// Валидируем дневные графики связанные с графиками работы
			if (scheduleSchemes.Any())
			{
				var errorLinksWithScheduleSchemes = new StringBuilder();
				foreach (var scheduleScheme in scheduleSchemes)
				{
					switch (scheduleScheme.Type)
					{
						case ScheduleSchemeType.Week:
							break;
						case ScheduleSchemeType.Month:
						case ScheduleSchemeType.SlideDay:
							if (dayInterval.SlideTime != TimeSpan.Zero)
								errorLinksWithScheduleSchemes.AppendLine(String.Format("{0} ({1}{2})", scheduleScheme.Name, scheduleScheme.Type.ToDescription().ToLower(), scheduleScheme.IsDeleted ? ", архивный" : null));
							break;
					}
				}
				if (errorLinksWithScheduleSchemes.Length > 0)
				{
					var error =
						String.Format(
							"Чтобы задать в поле \"Обязательная продолжительность скользящего графика\" ненулевое значение, необходимо удалить связи дневного графика со следующими сменными или месячными графиками:\n\n{0}",
							errorLinksWithScheduleSchemes);
					return new OperationResult(error);
				}
			}

			return ValidateDayIntervalParts(dayInterval);
		}

		private static OperationResult ValidateDayIntervalParts(DayInterval dayInterval)
		{
			var generalTimeSpan = TimeSpan.Zero;
			
			foreach (var dayIntervalPart in dayInterval.DayIntervalParts)
			{
				if (dayIntervalPart.TransitionType == DayIntervalPartTransitionType.Night)
					return new OperationResult("График содержит интервалы с переходом. Для сохранения сведений о дневном графике задайте значение 0 в поле \"Обязательная продолжительность скользящего графика\".\n\nЕсли вы хотите задать в поле \"Обязательная продолжительность скользящего графика\" ненулевое значение, сначала удалите интервалы с переходом, а затем введите необходимое значение");
				generalTimeSpan = generalTimeSpan.Add(dayIntervalPart.EndTime.Subtract(dayIntervalPart.BeginTime));
			}

			if (generalTimeSpan < dayInterval.SlideTime)
			{
				var error =
					String.Format(
						"Суммарная продолжительность интервалов дневного графика ({0}) должна быть больше или равна обязательной продолжительности скользящего графика ({1})",
						String.Format("{0} ч {1} мин", generalTimeSpan.Hours, generalTimeSpan.Minutes),
						String.Format("{0} ч {1} мин", dayInterval.SlideTime.Hours, dayInterval.SlideTime.Minutes));
				return new OperationResult(error);
			}

			return new OperationResult();
		}
	}
}
