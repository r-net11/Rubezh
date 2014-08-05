using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using LinqKit;

namespace SKDDriver
{
	public class AccessTemplateTranslator : OrganisationElementTranslator<DataAccess.AccessTemplate, AccessTemplate, AccessTemplateFilter>
	{
		public AccessTemplateTranslator(DataAccess.SKDDataContext context, CardDoorTranslator cardDoorTranslator)
			: base(context)
		{
			CardDoorTranslator = cardDoorTranslator;
		}

		CardDoorTranslator CardDoorTranslator;

		protected override OperationResult CanSave(AccessTemplate item)
		{
			bool sameName = Table.Any(x => x.Name == item.Name &&
				x.OrganisationUID == item.OrganisationUID &&
				x.UID != item.UID &&
				!x.IsDeleted);
			if (sameName)
				return new OperationResult("Попытка добавить шаблон доступа с совпадающим именем");
			return base.CanSave(item);
		}

		protected override OperationResult CanDelete(Guid uid)
		{
			if (Context.Cards.Any(x => x.AccessTemplateUID == uid &&
					x.IsDeleted == false))
				return new OperationResult("Невозможно удалить шаблон доступа, пока он указан у действующих карт");
			return base.CanDelete(uid);
		}

		public override OperationResult MarkDeleted(Guid uid)
		{
			var deleteDoorsResult = CardDoorTranslator.MarkDeletefFromAccessTemplate(uid);
			if (deleteDoorsResult.HasError)
				return deleteDoorsResult;
			return base.MarkDeleted(uid);
		}

		protected override AccessTemplate Translate(DataAccess.AccessTemplate tableItem)
		{
			var result = base.Translate(tableItem);
			result.CardDoors = CardDoorTranslator.GetForAccessTemplate(tableItem.UID);
			result.Name = tableItem.Name;
			var guardZones = (from x in Context.GuardZones.Where(x => x.ParentUID == tableItem.UID) select x);
			foreach (var item in guardZones)
			{
				result.GuardZoneAccesses.Add(new XGuardZoneAccess
				{
					ZoneUID = item.ZoneUID,
					CanReset = item.CanReset,
					CanSet = item.CanSet
				});
			}
			return result;
		}

		protected override void TranslateBack(DataAccess.AccessTemplate tableItem, AccessTemplate apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Name = apiItem.Name;
		}

		public override OperationResult Save(AccessTemplate item)
		{
			var updateCardDoorsResult = CardDoorTranslator.RemoveFromAccessTemplate(item);
			var result = base.Save(item);
			CardDoorTranslator.Save(item.CardDoors);
			return result;
		}

		protected override Expression<Func<DataAccess.AccessTemplate, bool>> IsInFilter(AccessTemplateFilter filter)
		{
			var result = base.IsInFilter(filter);
			var names = filter.Names;
			if (names != null && names.Count != 0)
				result = result.And(e => names.Contains(e.Name));
			return result;
		}

		public OperationResult SaveGuardZones(AccessTemplate apiItem)
		{
			return SaveGuardZonesInternal(apiItem.UID, apiItem.GuardZoneAccesses);
		}

		OperationResult SaveGuardZonesInternal(Guid parentUID, List<XGuardZoneAccess> GuardZones)
		{
			try
			{
				var tableOrganisationGuardZones = Context.GuardZones.Where(x => x.ParentUID == parentUID);
				Context.GuardZones.DeleteAllOnSubmit(tableOrganisationGuardZones);
				foreach (var guardZone in GuardZones)
				{
					var tableOrganisationGuardZone = new DataAccess.GuardZone();
					tableOrganisationGuardZone.UID = Guid.NewGuid();
					tableOrganisationGuardZone.ParentUID = parentUID;
					tableOrganisationGuardZone.ZoneUID = guardZone.ZoneUID;
					tableOrganisationGuardZone.CanSet = guardZone.CanSet;
					tableOrganisationGuardZone.CanReset = guardZone.CanReset;
					Context.GuardZones.InsertOnSubmit(tableOrganisationGuardZone);
				}
				Table.Context.SubmitChanges();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
			return new OperationResult();
		}
	}
}