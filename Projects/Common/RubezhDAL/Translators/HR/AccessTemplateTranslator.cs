using System;
using System.Data.Entity;
using System.Linq;
using RubezhAPI;
using API = RubezhAPI.SKD;
using System.Collections.Generic;

namespace RubezhDAL.DataClasses
{
	public class AccessTemplateTranslator : OrganisationItemTranslatorBase<AccessTemplate, API.AccessTemplate, API.AccessTemplateFilter>
	{
		public AccessTemplateTranslator(DbService context)
			: base(context) { }

		public override DbSet<AccessTemplate> Table
		{
			get { return Context.AccessTemplates; }
		}

		public override IQueryable<AccessTemplate> GetTableItems()
		{
			return base.GetTableItems().Include(x => x.CardDoors);
		}

		protected override OperationResult CanDelete(Guid uid)
		{
			if (Context.Cards.Any(x => x.AccessTemplateUID == uid))
				return new OperationResult("Невозможно удалить шаблон доступа, пока он указан у действующих карт");
			return base.CanDelete(uid);
		}

		public override void TranslateBack(API.AccessTemplate apiItem, AccessTemplate tableItem)
		{
			base.TranslateBack(apiItem, tableItem);
			tableItem.CardDoors = apiItem.CardDoors.Select(x => new CardDoor(x)).ToList();
		}

		protected override void ClearDependentData(AccessTemplate tableItem)
		{
			Context.CardDoors.RemoveRange(tableItem.CardDoors);
		}

		protected override IEnumerable<API.AccessTemplate> GetAPIItems(IQueryable<AccessTemplate> tableItems)
		{
			return tableItems.Select(tableItem => new API.AccessTemplate
			{
				UID = tableItem.UID,
				Name = tableItem.Name,
				Description = tableItem.Description,
				IsDeleted = tableItem.IsDeleted,
				RemovalDate = tableItem.RemovalDate != null ? tableItem.RemovalDate.Value : new DateTime(),
				OrganisationUID = tableItem.OrganisationUID != null ? tableItem.OrganisationUID.Value : Guid.Empty,
				CardDoors = tableItem.CardDoors.Select(x => new RubezhAPI.SKD.CardDoor
				{
					UID = x.UID,
					CardUID = x.CardUID,
					DoorUID = x.DoorUID,
					AccessTemplateUID = x.AccessTemplateUID,
					EnterScheduleNo = x.ExitScheduleNo,
					ExitScheduleNo = x.EnterScheduleNo
				}).ToList()
			});
		}
	}
}