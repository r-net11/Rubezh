using System;
using System.Linq;
using FiresecAPI;
using System.Data.Linq;
using LinqKit;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace SKDDriver
{
	public class CardsTranslator:TranslatorBase<DataAccess.Card, SKDCard, CardFilter>
	{
		public CardsTranslator(Table<DataAccess.Card> table, DataAccess.SKUDDataContext context)
			: base(table, context)
		{

		}

		protected override OperationResult CanSave(SKDCard item)
		{
			bool sameSeriesNo = Table.Any(x => x.Number == item.Number &&
				x.Series == item.Series);
			if (sameSeriesNo)
				return new OperationResult("Попытка добавления карты с совпадающей комбинацией серии и номера");
			return base.CanSave(item);
		}

		protected override OperationResult CanDelete(SKDCard item)
		{
			if (Context.Employee.Any(x => x.Uid == item.HolderUid &&
					x.IsDeleted == true))
				return new OperationResult("Не могу удалить карту, пока она указана у действующих сотрудников");
			return base.CanSave(item);
		}

		public override OperationResult MarkDeleted(IEnumerable<SKDCard> items)
		{
			//try
			//{
			//    foreach (var item in items)
			//    {
			//        if (item == null)
			//            continue;
			//        foreach (var cardZone in item.CardZones)
			//        {
			//            if (cardZone != null)
			//                continue;
			//            var databaseItem = Context.CardZoneLink.FirstOrDefault(x => x.Uid == cardZone.UID);
			//            if (databaseItem != null)
			//            {
			//                databaseItem.IsDeleted = true;
			//                databaseItem.RemovalDate = DateTime.Now;
			//            }
			//        }
			//    }
			//    Context.SubmitChanges();
			//}
			//catch(Exception e)
			//{
			//    return new OperationResult(e.Message);
			//}
			return base.MarkDeleted(items);
		}




		protected override SKDCard Translate(DataAccess.Card tableItem)
		{
			var result = base.Translate(tableItem);
			var zoneUids = new List<Guid>();
			foreach (var cardZoneLink in tableItem.CardZoneLink)
			{
				if (cardZoneLink.ZoneUid != null)
					zoneUids.Add(cardZoneLink.ZoneUid.Value);
			}
			result.HolderUid = tableItem.EmployeeUid;
			result.Number = tableItem.Number;
			result.Series = tableItem.Series;
			result.ValidFrom = tableItem.ValidFrom;
			result.ValidTo = tableItem.ValidTo;
			result.ZoneLinkUids = zoneUids;
			result.IsAntipass = tableItem.IsAntipass;
			result.IsInStopList = tableItem.IsInStopList;
			result.StopReason = tableItem.StopReason;
			return result;
		}

		protected override DataAccess.Card TranslateBack(SKDCard apiItem)
		{
			var result = base.TranslateBack(apiItem);
			result.EmployeeUid = apiItem.HolderUid;
			result.Number = apiItem.Number;
			result.Series = apiItem.Series;
			result.ValidFrom = CheckDate(apiItem.ValidFrom);
			result.ValidTo = CheckDate(apiItem.ValidTo);
			result.IsAntipass = apiItem.IsAntipass;
			result.IsInStopList = apiItem.IsInStopList;
			result.StopReason = apiItem.StopReason;
			return result;
		}

		protected override void Update(DataAccess.Card tableItem, SKDCard apiItem)
		{
			base.Update(tableItem, apiItem);
			tableItem.Number = apiItem.Number;
			tableItem.Series = apiItem.Series;
			tableItem.EmployeeUid = apiItem.HolderUid;
			tableItem.ValidFrom = CheckDate(apiItem.ValidFrom);
			tableItem.ValidTo = CheckDate(apiItem.ValidTo);
			tableItem.IsAntipass = apiItem.IsAntipass;
			tableItem.IsInStopList = apiItem.IsInStopList;
			tableItem.StopReason = apiItem.StopReason;
		}

		protected override Expression<Func<DataAccess.Card, bool>> IsInFilter(CardFilter filter)
		{
			var result = PredicateBuilder.True<DataAccess.Card>();
			result = result.And(base.IsInFilter(filter));
			var employeeUIDs = filter.EmployeeUids;
			if (employeeUIDs != null && employeeUIDs.Count != 0)
				result = result.And(e => e.EmployeeUid.HasValue && employeeUIDs.Contains(e.EmployeeUid.Value));
			return result;
		}
	}
}
