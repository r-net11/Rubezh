using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI;
using LinqKit;

namespace SKDDriver
{
	public class OrganisationTranslator : IsDeletedTranslator<DataAccess.Organisation, Organisation, OrganisationFilter>
	{
		public OrganisationTranslator(DataAccess.SKDDataContext context)
			: base(context)
		{
			;
		}

		protected override OperationResult CanSave(Organisation item)
		{
			bool sameName = Table.Any(x => x.Name == item.Name);
			if (sameName)
				return new OperationResult("Организация таким же именем уже содержится в базе данных");
			return base.CanSave(item);
		}

		protected override OperationResult CanDelete(Organisation item)
		{
			var uid = item.UID;
			if (Context.AdditionalColumnTypes.Any(x => x.OrganisationUID == uid) ||
					Context.Departments.Any(x => x.OrganisationUID == uid) ||
					Context.Documents.Any(x => x.OrganisationUID == uid) ||
					Context.Employees.Any(x => x.OrganisationUID == uid) ||
					Context.EmployeeReplacements.Any(x => x.OrganisationUID == uid) ||
					Context.Holidays.Any(x => x.OrganisationUID == uid) ||
					Context.NamedIntervals.Any(x => x.OrganisationUID == uid) ||
					Context.Positions.Any(x => x.OrganisationUID == uid) ||
					Context.Phones.Any(x => x.OrganisationUID == uid) ||
					Context.Schedules.Any(x => x.OrganisationUID == uid) ||
					Context.ScheduleSchemes.Any(x => x.OrganisationUID == uid) ||
					Context.AccessTemplates.Any(x => x.OrganisationUID == uid)
				)
				return new OperationResult("Организация не может быть удалена, пока существуют элементы привязанные к ней");
			return base.CanSave(item);
		}

		protected override Organisation Translate(DataAccess.Organisation tableItem)
		{
			var result = base.Translate(tableItem);
			result.Name = tableItem.Name;
			result.Description = tableItem.Description;
			result.PhotoUID = tableItem.PhotoUID;
			result.ZoneUIDs = (from x in Context.OrganisationZones.Where(x => x.OrganisationUID == result.UID) select x.ZoneUID).ToList();
			return result;
		}

		protected override void TranslateBack(DataAccess.Organisation tableItem, Organisation apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Name = apiItem.Name;
			tableItem.Description = apiItem.Description;
			tableItem.PhotoUID = apiItem.PhotoUID;
		}

		protected override Expression<Func<DataAccess.Organisation, bool>> IsInFilter(OrganisationFilter filter)
		{
			var result = PredicateBuilder.True<DataAccess.Organisation>();
			result = result.And(base.IsInFilter(filter));
			return result;
		}

		public OperationResult SaveZones(Organisation apiItem)
		{
			try
			{
				var zoneUIDs = apiItem.ZoneUIDs;
				var tableOrganisationZones = Context.OrganisationZones.Where(x => x.OrganisationUID == apiItem.UID);
				Context.OrganisationZones.DeleteAllOnSubmit(tableOrganisationZones);
				foreach (var zoneUID in apiItem.ZoneUIDs)
				{
					var tableOrganisationZone = new DataAccess.OrganisationZone();
					tableOrganisationZone.UID = Guid.NewGuid();
					tableOrganisationZone.OrganisationUID = apiItem.UID;
					tableOrganisationZone.ZoneUID = zoneUID;
					Context.OrganisationZones.InsertOnSubmit(tableOrganisationZone);
				}
				Table.Context.SubmitChanges();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
			return new OperationResult();
		}

		public override OperationResult Save(IEnumerable<Organisation> apiItems)
		{
			if (apiItems == null)
				return new OperationResult();
			foreach (var apiItem in apiItems)
			{
				SaveZones(apiItem);
			}
			return base.Save(apiItems);
		}
	}
}