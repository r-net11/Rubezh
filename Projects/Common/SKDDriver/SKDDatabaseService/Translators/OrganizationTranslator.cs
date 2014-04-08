using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI;
using LinqKit;

namespace SKDDriver
{
	public class OrganizationTranslator : IsDeletedTranslator<DataAccess.Organization, Organization, OrganizationFilter>
	{
		public OrganizationTranslator(DataAccess.SKDDataContext context)
			: base(context)
		{
			;
		}

		protected override OperationResult CanSave(Organization item)
		{
			bool sameName = Table.Any(x => x.Name == item.Name);
			if (sameName)
				return new OperationResult("Организация таким же именем уже содержится в базе данных");
			return base.CanSave(item);
		}

		protected override OperationResult CanDelete(Organization item)
		{
			var uid = item.UID;
			if (Context.AdditionalColumnTypes.Any(x => x.OrganizationUID == uid) ||
					Context.Departments.Any(x => x.OrganizationUID == uid) ||
					Context.Documents.Any(x => x.OrganizationUID == uid) ||
					Context.Employees.Any(x => x.OrganizationUID == uid) ||
					Context.EmployeeReplacements.Any(x => x.OrganizationUID == uid) ||
					Context.Holidays.Any(x => x.OrganizationUID == uid) ||
					Context.NamedIntervals.Any(x => x.OrganizationUID == uid) ||
					Context.Positions.Any(x => x.OrganizationUID == uid) ||
					Context.Phones.Any(x => x.OrganizationUID == uid) ||
					Context.Schedules.Any(x => x.OrganizationUID == uid) ||
					Context.ScheduleSchemes.Any(x => x.OrganizationUID == uid) ||
					Context.AccessTemplates.Any(x => x.OrganizationUID == uid)
				)
				return new OperationResult("Организация не может быть удалена, пока существуют элементы привязанные к ней");
			return base.CanSave(item);
		}

		protected override Organization Translate(DataAccess.Organization tableItem)
		{
			var result = base.Translate(tableItem);
			result.Name = tableItem.Name;
			result.Description = tableItem.Description;
			result.PhotoUID = tableItem.PhotoUID;
			result.ZoneUIDs = (from x in Context.OrganizationZones.Where(x => x.OrganizationUID == result.UID) select x.ZoneUID).ToList();
			return result;
		}

		protected override void TranslateBack(DataAccess.Organization tableItem, Organization apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Name = apiItem.Name;
			tableItem.Description = apiItem.Description;
			tableItem.PhotoUID = apiItem.PhotoUID;
		}

		protected override Expression<Func<DataAccess.Organization, bool>> IsInFilter(OrganizationFilter filter)
		{
			var result = PredicateBuilder.True<DataAccess.Organization>();
			result = result.And(base.IsInFilter(filter));
			return result;
		}

		public OperationResult SaveZones(Organization apiItem)
		{
			try
			{
				var zoneUIDs = apiItem.ZoneUIDs;
				var tableOrganizationZones = Context.OrganizationZones.Where(x => x.OrganizationUID == apiItem.UID);
				Context.OrganizationZones.DeleteAllOnSubmit(tableOrganizationZones);
				foreach (var zoneUID in apiItem.ZoneUIDs)
				{
					var tableOrganizationZone = new DataAccess.OrganizationZone();
					tableOrganizationZone.UID = Guid.NewGuid();
					tableOrganizationZone.OrganizationUID = apiItem.UID;
					tableOrganizationZone.ZoneUID = zoneUID;
					Context.OrganizationZones.InsertOnSubmit(tableOrganizationZone);
				}
				Table.Context.SubmitChanges();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
			return new OperationResult();
		}

		public override OperationResult Save(IEnumerable<Organization> apiItems)
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