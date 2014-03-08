using System;
using System.Linq;
using FiresecAPI;
using System.Data.Linq;
using LinqKit;
using System.Linq.Expressions;

namespace SKDDriver
{
	public class JournalItemTranslator : TranslatorBase<DataAccess.Journal, SKDJournalItem, SKDJournalFilter>
	{
		public JournalItemTranslator(Table<DataAccess.Journal> table, DataAccess.SKUDDataContext context)
			: base(table, context)
		{
			;
		}

		protected override SKDJournalItem Translate(DataAccess.Journal tableItem)
		{
			var result = base.Translate(tableItem);
			result.Name = tableItem.Name;
			result.Description = tableItem.Description;
			result.SystemDateTime = tableItem.SysemDate;
			result.DeviceDateTime = tableItem.DeviceDate;
			result.CardNo = tableItem.CardNo;
			result.CardSeries = tableItem.CardSeries;
			result.CardUid = tableItem.CardUid;
			result.DeviceJournalRecordNo = tableItem.DeviceNo;
			result.IpAddress = tableItem.IpPort;
			return result;
		}

		protected override void TranslateBack(DataAccess.Journal tableItem, SKDJournalItem apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.CardNo = apiItem.CardNo;
			tableItem.Description = apiItem.Description;
			tableItem.DeviceDate = apiItem.DeviceDateTime;
			tableItem.DeviceNo = apiItem.DeviceJournalRecordNo;
			tableItem.IpPort = apiItem.IpAddress;
			tableItem.Name = apiItem.Name;
			tableItem.SysemDate = CheckDate(apiItem.SystemDateTime.GetValueOrDefault(new DateTime()));
		}

		protected override Expression<Func<DataAccess.Journal, bool>> IsInFilter(SKDJournalFilter filter)
		{
			var result = PredicateBuilder.True<DataAccess.Journal>();
			result = result.And(base.IsInFilter(filter));

			var eventNames = filter.EventNames;
			if (eventNames != null && eventNames.Count != 0)
				result = result.And(e => eventNames.Contains(e.Name));

			var deviceDates = filter.DeviceDateTime;
			if (deviceDates != null)
				result = result.And(e => e.DeviceDate != null &&
					e.DeviceDate >= deviceDates.StartDate &&
					e.DeviceDate <= deviceDates.EndDate);

			var systemDates = filter.SystemDateTime;
			if (systemDates != null)
				result = result.And(e => e.SysemDate != null &&
					e.SysemDate >= systemDates.StartDate &&
					e.SysemDate <= systemDates.EndDate);

			return result;
		}
	}
}
