﻿using System;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI;
using FiresecAPI.SKD;
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

		protected override OperationResult CanDelete(Guid uid)
		{
			if (Context.Cards.Any(x => x.AccessTemplateUID == uid &&
					x.IsDeleted == false))
				return new OperationResult("Не могу удалить ГУД, пока он указан у действующих карт");
			return base.CanDelete(uid);
		}

		public override OperationResult MarkDeleted(Guid uid)
		{
			var deleteZonesResult = CardZonesTranslator.MarkDeletefFromAccessTemplate(uid);
			if (deleteZonesResult.HasError)
				return deleteZonesResult;
			return base.MarkDeleted(uid);
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

		public override OperationResult Save(AccessTemplate item)
		{
			var updateZonesResult = CardZonesTranslator.SaveFromAccessTemplate(item);
			if (updateZonesResult.HasError)
				return updateZonesResult;
			return base.Save(item);
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