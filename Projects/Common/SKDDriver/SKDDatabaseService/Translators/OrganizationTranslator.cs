using System;
using System.Linq;
using FiresecAPI;
using System.Data.Linq;
using LinqKit;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace SKDDriver
{
	public class OrganizationTranslator : TranslatorBase<DataAccess.Organization, Organization, OrganizationFilter>
	{
		public OrganizationTranslator(Table<DataAccess.Organization> table, DataAccess.SKUDDataContext context)
			: base(table, context)
		{
			;
		}

		CardZonesTranslator CardZonesTranslator;

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
			if (Context.AdditionalColumn.Any(x => x.OrganizationUid == uid) ||
					Context.Day.Any(x => x.OrganizationUid == uid) ||
					Context.Department.Any(x => x.OrganizationUid == uid) ||
					Context.Document.Any(x => x.OrganizationUid == uid) ||
					Context.Employee.Any(x => x.OrganizationUid == uid) ||
					Context.EmployeeReplacement.Any(x => x.OrganizationUid == uid) ||
					Context.Holiday.Any(x => x.OrganizationUid == uid) ||
					Context.NamedInterval.Any(x => x.OrganizationUid == uid) ||
					Context.Position.Any(x => x.OrganizationUid == uid) ||
					Context.Phone.Any(x => x.OrganizationUid == uid) ||
					Context.Schedule.Any(x => x.OrganizationUid == uid) ||
					Context.ScheduleScheme.Any(x => x.OrganizationUid == uid) ||
					Context.GUD.Any(x => x.OrganizationUid == uid))
				return new OperationResult("Организация не может быть удалена, пока существуют элементы привязанные к ней");
			return base.CanSave(item);
		}

		protected override Organization Translate(DataAccess.Organization tableItem)
		{
			var result = base.Translate(tableItem);
			result.Name = tableItem.Name;
			result.Description = tableItem.Description;
			return result;
		}

		protected override void Update(DataAccess.Organization tableItem, Organization apiItem)
		{
			base.Update(tableItem, apiItem);
			tableItem.Name = apiItem.Name;
			tableItem.Description = apiItem.Description;
		}

		protected override Expression<Func<DataAccess.Organization, bool>> IsInFilter(OrganizationFilter filter)
		{
			var result = PredicateBuilder.True<DataAccess.Organization>();
			result = result.And(base.IsInFilter(filter));
			return result;
		}
	}
}
