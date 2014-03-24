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
			if (Context.AdditionalColumnType.Any(x => x.OrganizationUID == uid) ||
					Context.Day.Any(x => x.OrganizationUID == uid) ||
					Context.Department.Any(x => x.OrganizationUID == uid) ||
					Context.Document.Any(x => x.OrganizationUID == uid) ||
					Context.Employee.Any(x => x.OrganizationUID == uid) ||
					Context.EmployeeReplacement.Any(x => x.OrganizationUID == uid) ||
					Context.Holiday.Any(x => x.OrganizationUID == uid) ||
					Context.NamedInterval.Any(x => x.OrganizationUID == uid) ||
					Context.Position.Any(x => x.OrganizationUID == uid) ||
					Context.Phone.Any(x => x.OrganizationUID == uid) ||
					Context.Schedule.Any(x => x.OrganizationUID == uid) ||
					Context.ScheduleScheme.Any(x => x.OrganizationUID == uid) ||
					Context.AccessTemplate.Any(x => x.OrganizationUID == uid)
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
			result.ZoneUIDs = (from x in Context.OrganizationZone.Where(x => x.OrganizationUID == result.UID) select x.ZoneUID).ToList();
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
				var tableOrganizationZones = Context.OrganizationZone.Where(x => x.OrganizationUID == apiItem.UID);
				Context.OrganizationZone.DeleteAllOnSubmit(tableOrganizationZones);
				foreach (var zoneUID in apiItem.ZoneUIDs)
				{
					var tableOrganizationZone = new DataAccess.OrganizationZone();
					tableOrganizationZone.UID = Guid.NewGuid();
					tableOrganizationZone.OrganizationUID = apiItem.UID;
					tableOrganizationZone.ZoneUID = zoneUID;
					Context.OrganizationZone.InsertOnSubmit(tableOrganizationZone);
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