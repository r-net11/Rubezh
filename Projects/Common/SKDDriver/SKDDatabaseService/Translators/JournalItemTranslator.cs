using System;
using System.Linq;
using FiresecAPI;
using System.Data.Linq;
using LinqKit;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace SKDDriver
{
	public class JournalItemTranslator : TranslatorBase<DataAccess.Journal, SKDJournalItem, SKDJournalFilter>
	{
		public JournalItemTranslator(DataAccess.SKUDDataContext context)
			: base(context)
		{
			;
		}

		protected override SKDJournalItem Translate(DataAccess.Journal tableItem)
		{
			var result = new SKDJournalItem();
			result.UID = tableItem.UID;
			result.Name = tableItem.Name;
			result.Description = tableItem.Description;
			result.SystemDateTime = tableItem.SysemDate;
			result.DeviceDateTime = tableItem.DeviceDate;
			result.CardNo = tableItem.CardNo;
			result.CardSeries = tableItem.CardSeries;
			result.CardUID = tableItem.CardUID;
			result.DeviceJournalRecordNo = tableItem.DeviceNo;
			result.IpAddress = tableItem.IpPort;
			return result;
		}

		protected override void TranslateBack(DataAccess.Journal tableItem, SKDJournalItem apiItem)
		{
			tableItem.CardNo = apiItem.CardNo;
			tableItem.Description = apiItem.Description;
			tableItem.DeviceDate = apiItem.DeviceDateTime;
			tableItem.DeviceNo = apiItem.DeviceJournalRecordNo;
			tableItem.IpPort = apiItem.IpAddress;
			tableItem.Name = apiItem.Name;
			tableItem.SysemDate = CheckDate(apiItem.SystemDateTime);
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

		public OperationResult Remove(IEnumerable<SKDJournalItem> items)
		{
			var operationResult = new OperationResult();
			try
			{
				foreach (var item in items)
				{
					var verifyResult = CanDelete(item);
					if (verifyResult.HasError)
						return verifyResult;
					if (item != null)
					{
						var databaseItem = (from x in Table where x.UID.Equals(item.UID) select x).FirstOrDefault();
						if (databaseItem != null)
						{
							Table.DeleteOnSubmit(databaseItem);
						}
					}
				}
				Table.Context.SubmitChanges();
				return operationResult;
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

	}
}
