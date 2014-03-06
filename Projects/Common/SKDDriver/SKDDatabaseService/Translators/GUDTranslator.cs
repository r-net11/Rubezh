using System;
using System.Linq;
using FiresecAPI;
using System.Data.Linq;
using LinqKit;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace SKDDriver
{
	public class GUDTranslator:TranslatorBase<DataAccess.GUD, GUD, GUDFilter>
	{
		public GUDTranslator(Table<DataAccess.GUD> table, DataAccess.SKUDDataContext context, CardZoneTranslator cardsTranslator)
			: base(table, context)
		{
			CardZonesTranslator = cardsTranslator;
		}

		CardZoneTranslator CardZonesTranslator;

		protected override OperationResult CanSave(GUD item)
		{
			bool sameName = Table.Any(x => x.Name == item.Name && 
				x.OrganizationUid == item.OrganizationUid &&
				x.UID != item.UID &&
				!x.IsDeleted);
			if (sameName)
				return new OperationResult("Попытка добавить ГУД с совпадающим именем");
			return base.CanSave(item);
		}

		protected override OperationResult CanDelete(GUD item)
		{
			if (Context.Card.Any(x => x.GUDUid == item.UID &&
					x.IsDeleted == false))
				return new OperationResult("Не могу удалить ГУД, пока он указан у действующих карт");
			return base.CanSave(item);
		}

		public override OperationResult MarkDeleted(IEnumerable<GUD> items)
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

		protected override GUD Translate(DataAccess.GUD tableItem) 
		{
			var result = base.Translate(tableItem);
			result.CardZones = CardZonesTranslator.Get(tableItem.UID, ParentType.GUD);
			result.Name = tableItem.Name;
			return result;
		}

		protected override void TranslateBack(DataAccess.GUD tableItem, GUD apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Name = apiItem.Name;
		}

		public override OperationResult Save(IEnumerable<GUD> items)
		{
			var updateZonesResult = CardZonesTranslator.SaveFromGUDs(items);
			if (updateZonesResult.HasError)
				return updateZonesResult;
			return base.Save(items);
		}

		protected override Expression<Func<DataAccess.GUD, bool>> IsInFilter(GUDFilter filter)
		{
			var result = PredicateBuilder.True<DataAccess.GUD>();
			result = result.And(base.IsInFilter(filter));
			var names = filter.Names;
			if (names != null && names.Count != 0)
				result = result.And(e => names.Contains(e.Name));
			return result;
		}
	}
}
