using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI;
using LinqKit;

namespace SKDDriver
{
	public class AccessTemplateTranslator : OrganisationElementTranslator<DataAccess.AccessTemplate, AccessTemplate, AccessTemplateFilter>
	{
		public AccessTemplateTranslator(DataAccess.SKDDataContext context, CardZoneTranslator cardsTranslator)
			: base(context)
		{
			CardZonesTranslator = cardsTranslator;
		}

		CardZoneTranslator CardZonesTranslator;

		protected override OperationResult CanSave(AccessTemplate item)
		{
			bool sameName = Table.Any(x => x.Name == item.Name &&
				x.OrganisationUID == item.OrganisationUID &&
				x.UID != item.UID &&
				!x.IsDeleted);
			if (sameName)
				return new OperationResult("Попытка добавить ГУД с совпадающим именем");
			return base.CanSave(item);
		}

		protected override OperationResult CanDelete(AccessTemplate item)
		{
			if (Context.Cards.Any(x => x.AccessTemplateUID == item.UID &&
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

		protected override AccessTemplate Translate(DataAccess.AccessTemplate tableItem)
		{
			var result = base.Translate(tableItem);
			result.CardZones = CardZonesTranslator.Get(tableItem.UID);
			result.Name = tableItem.Name;
			return result;
		}

		protected override void TranslateBack(DataAccess.AccessTemplate tableItem, AccessTemplate apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Name = apiItem.Name;
		}

		public override OperationResult Save(IEnumerable<AccessTemplate> items)
		{
			var updateZonesResult = CardZonesTranslator.SaveFromAccessTemplates(items);
			if (updateZonesResult.HasError)
				return updateZonesResult;
			return base.Save(items);
		}

		protected override Expression<Func<DataAccess.AccessTemplate, bool>> IsInFilter(AccessTemplateFilter filter)
		{
			var result = PredicateBuilder.True<DataAccess.AccessTemplate>();
			result = result.And(base.IsInFilter(filter));
			var names = filter.Names;
			if (names != null && names.Count != 0)
				result = result.And(e => names.Contains(e.Name));
			return result;
		}
	}
}