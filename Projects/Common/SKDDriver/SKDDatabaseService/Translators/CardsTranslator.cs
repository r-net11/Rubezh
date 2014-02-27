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
		public CardsTranslator(Table<DataAccess.Card> table, DataAccess.SKUDDataContext context, CardZonesTranslator cardsTranslator)
			: base(table, context)
		{
			CardZonesTranslator = cardsTranslator;
		}

		CardZonesTranslator CardZonesTranslator;

		protected override OperationResult CanSave(SKDCard item)
		{
			bool sameSeriesNo = Table.Any(x => x.Number == item.Number &&
				x.Series == item.Series &&
				x.Uid != item.UID);
			if (sameSeriesNo)
				return new OperationResult("Попытка добавить карту с повторяющейся комбинацией серии и номера");
			foreach (var cardZone in item.AdditionalGUDZones)
			{
				if(item.ExceptedGUDZones.Any(x => x.ZoneUID == cardZone.ZoneUID))
					return new OperationResult("Попытка добавить одну и ту же зону как исключение и как расширение ГУД");
			}
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
			try
			{
				foreach (var item in items)
				{
					if (item == null)
						continue;
					CardZonesTranslator.MarkDeleted(item.CardZones);
				}
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
			return base.MarkDeleted(items);
		}

		protected override SKDCard Translate(DataAccess.Card tableItem) 
		{
			var result = base.Translate(tableItem);
			result.HolderUid = tableItem.EmployeeUid;
			result.Number = tableItem.Number;
			result.Series = tableItem.Series;
			result.ValidFrom = tableItem.ValidFrom;
			result.ValidTo = tableItem.ValidTo;
			result.GUDUid = tableItem.GUDUid;
			result.CardZones = CardZonesTranslator.Get(tableItem.Uid, ParentType.Card);
			result.AdditionalGUDZones = CardZonesTranslator.Get(tableItem.Uid, ParentType.GUDAdditions);
			result.ExceptedGUDZones = CardZonesTranslator.Get(tableItem.Uid, ParentType.GUDExceptons);
			result.IsAntipass = tableItem.IsAntipass;
			result.IsInStopList = tableItem.IsInStopList;
			result.StopReason = tableItem.StopReason;
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
			tableItem.GUDUid = apiItem.GUDUid;
		}

		public override OperationResult Save(IEnumerable<SKDCard> items)
		{
			var updateZonesResult = CardZonesTranslator.SaveFromCards(items);
			if (updateZonesResult.HasError)
				return updateZonesResult;
			return base.Save(items);
		}

		protected override Expression<Func<DataAccess.Card, bool>> IsInFilter(CardFilter filter)
		{
			var result = PredicateBuilder.True<DataAccess.Card>();
			result = result.And(base.IsInFilter(filter));

			var IsBlockedExpression = PredicateBuilder.True<DataAccess.Card>();
			switch (filter.WithBlocked)
			{
				case DeletedType.Deleted:
					IsBlockedExpression = e => e.IsInStopList;
					break;
				case DeletedType.Not:
					IsBlockedExpression = e => !e.IsInStopList;
					break;
				default:
					break;
			}
			result = result.And(IsBlockedExpression);

			var employeeUIDs = filter.EmployeeUids;
			if (employeeUIDs != null && employeeUIDs.Count != 0)
				result = result.And(e => e.EmployeeUid.HasValue && employeeUIDs.Contains(e.EmployeeUid.Value));
			return result;
		}
	}
}
