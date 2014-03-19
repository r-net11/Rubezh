using System;
using System.Linq;
using FiresecAPI;
using System.Data.Linq;
using LinqKit;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace SKDDriver
{
	public class GUDTranslator : IsDeletedTranslator<DataAccess.GUD, AccessTemplate, AccessTemplateFilter>
	{
		public GUDTranslator(DataAccess.SKUDDataContext context, CardZoneTranslator cardsTranslator)
			: base(context)
		{
			CardZonesTranslator = cardsTranslator;
		}

		CardZoneTranslator CardZonesTranslator;

		protected override OperationResult CanSave(AccessTemplate item)
		{
			bool sameName = Table.Any(x => x.Name == item.Name &&
				x.OrganizationUID == item.OrganizationUID &&
				x.UID != item.UID &&
				!x.IsDeleted);
			if (sameName)
				return new OperationResult("Попытка добавить ГУД с совпадающим именем");
			return base.CanSave(item);
		}

		protected override OperationResult CanDelete(AccessTemplate item)
		{
			if (Context.Card.Any(x => x.GUDUID == item.UID &&
					x.IsDeleted == false))
				return new OperationResult("Не могу удалить ГУД, пока он указан у действующих карт");
			return base.CanSave(item);
		}

		public override OperationResult MarkDeleted(IEnumerable<AccessTemplate> items)
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

		protected override AccessTemplate Translate(DataAccess.GUD tableItem)
		{
			var result = base.Translate(tableItem);
			result.CardZones = CardZonesTranslator.Get(tableItem.UID, ParentType.AccessTemplate);
			result.Name = tableItem.Name;
			return result;
		}

		protected override void TranslateBack(DataAccess.GUD tableItem, AccessTemplate apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Name = apiItem.Name;
		}

		public override OperationResult Save(IEnumerable<AccessTemplate> items)
		{
			var updateZonesResult = CardZonesTranslator.SaveFromGUDs(items);
			if (updateZonesResult.HasError)
				return updateZonesResult;
			return base.Save(items);
		}

		protected override Expression<Func<DataAccess.GUD, bool>> IsInFilter(AccessTemplateFilter filter)
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
