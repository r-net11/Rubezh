using System.Collections.Generic;
using FiresecAPI.SKD;
using System;
using System.Linq;
using OperationResult = FiresecAPI.OperationResult;

namespace SKDDriver.Translators
{
	public class DayIntervalTranslator : OrganisationElementTranslator<DataAccess.DayInterval, DayInterval, DayIntervalFilter>
	{
		public DayIntervalTranslator(SKDDatabaseService databaseService)
			: base(databaseService)
		{
		}

		protected override IQueryable<DataAccess.DayInterval> GetQuery(DayIntervalFilter filter)
		{
			return base.GetQuery(filter).OrderBy(item => item.Name);
		}

		protected override OperationResult CanSave(DayInterval item)
		{
			var result = base.CanSave(item);
			if (result.HasError)
				return result;
			bool hasSameName = Table.Any(x => x.Name == item.Name &&
				x.OrganisationUID == item.OrganisationUID &&
				x.UID != item.UID &&
				!x.IsDeleted);
			if (hasSameName)
				return new OperationResult("Дневной график с таким же названием уже существует");
			else
				return new OperationResult();
		}

		protected override DayInterval Translate(DataAccess.DayInterval tableItem)
		{
			var result = base.Translate(tableItem);
			result.Name = tableItem.Name;
			result.Description = tableItem.Description;
			result.SlideTime = TimeSpan.FromSeconds(tableItem.SlideTime);
			result.DayIntervalParts = DatabaseService.DayIntervalPartTranslator.TranslateAll(tableItem.DayIntervalParts.OrderBy(item => item.BeginTime));
			return result;
		}

		protected override void TranslateBack(DataAccess.DayInterval tableItem, DayInterval apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Name = apiItem.Name;
			tableItem.Description = apiItem.Description;
			tableItem.SlideTime = (int)apiItem.SlideTime.TotalSeconds;
			DatabaseService.DayIntervalPartTranslator.Save(apiItem.DayIntervalParts);
		}

		public DayInterval GetDayInterval(DayIntervalPart dayIntervalPart)
		{
			return Translate(Context.DayIntervals.FirstOrDefault(x => x.UID == dayIntervalPart.DayIntervalUID));
		}

		/// <summary>
		/// Получает коллекцию дневных графиков для схемы графика работы,
		/// отсортированную по порядку
		/// </summary>
		/// <param name="scheduleScheme">Схема графика работы</param>
		/// <returns>Коллекция дневных графиков</returns>
		public IEnumerable<DayInterval> GetDayIntervals(ScheduleScheme scheduleScheme)
		{
			var dayIntervalUids = Context.ScheduleDays.Where(x => x.ScheduleSchemeUID == scheduleScheme.UID)
				.OrderBy(y => y.Number)
				.Select(z => z.DayIntervalUID);


			var dayIntervals = (from guid in dayIntervalUids
					 select Context.DayIntervals.FirstOrDefault(x => x.UID == guid)
					 into result
					 select result
					 ).ToList();

			var translatedDayIntervals = dayIntervals.Select(dayInterval => dayInterval == null ? DayIntervalCreator.CreateDayIntervalNever() : Translate(dayInterval)).ToList();

			return translatedDayIntervals;
		}
	}
}