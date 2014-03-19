using System;
using System.Linq;
using FiresecAPI;
using System.Data.Linq;
using LinqKit;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace SKDDriver
{
	public class CardTranslator : IsDeletedTranslator<DataAccess.Card, SKDCard, CardFilter>
	{
		public CardTranslator(DataAccess.SKUDDataContext context, CardZoneTranslator cardsTranslator)
			: base(context)
		{
			CardZonesTranslator = cardsTranslator;
		}

		CardZoneTranslator CardZonesTranslator;

		protected override OperationResult CanSave(SKDCard item)
		{
			bool isSameSeriesNo = Table.Any(x => x.Number == item.Number &&
				x.Series == item.Series &&
				!x.IsDeleted &&
				x.UID != item.UID);
			if (isSameSeriesNo)
				return new OperationResult("Попытка добавить карту с повторяющейся комбинацией серии и номера");
			return base.CanSave(item);
		}

		protected override OperationResult CanDelete(SKDCard item)
		{
			if (Context.Employee.Any(x => x.UID == item.HolderUID &&
					!x.IsDeleted))
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
			result.HolderUID = tableItem.EmployeeUID;
			result.Number = tableItem.Number;
			result.Series = tableItem.Series;
			result.ValidFrom = tableItem.ValidFrom;
			result.ValidTo = tableItem.ValidTo;
			result.AccessTemplateUID = tableItem.GUDUID;
			result.CardZones = CardZonesTranslator.Get(tableItem.UID, ParentType.Card);
			result.IsInStopList = tableItem.IsInStopList;
			result.StopReason = tableItem.StopReason;
			return result;
		}

		protected override void TranslateBack(DataAccess.Card tableItem, SKDCard apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Number = apiItem.Number;
			tableItem.Series = apiItem.Series;
			tableItem.EmployeeUID = apiItem.HolderUID;
			tableItem.ValidFrom = CheckDate(apiItem.ValidFrom);
			tableItem.ValidTo = CheckDate(apiItem.ValidTo);
			tableItem.IsInStopList = apiItem.IsInStopList;
			tableItem.StopReason = apiItem.StopReason;
			tableItem.GUDUID = apiItem.AccessTemplateUID;
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

			var employeeUIDs = filter.EmployeeUIDs;
			if (employeeUIDs != null && employeeUIDs.Count != 0)
				result = result.And(e => e.EmployeeUID.HasValue && employeeUIDs.Contains(e.EmployeeUID.Value));
			return result;
		}
	}
}