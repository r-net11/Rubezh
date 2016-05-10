using StrazhAPI;
using StrazhAPI.SKD;
using LinqKit;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace StrazhDAL
{
	public abstract class EmployeeTranslatorBase<TTableType, TApiType, TFilter, TShort> : WithShortTranslator<TTableType, TApiType, TFilter, TShort>
		where TTableType : class, DataAccess.IOrganisationDatabaseElement, DataAccess.IDatabaseElement, DataAccess.IIsDeletedDatabaseElement, new()
		where TApiType : OrganisationElementBase, new()
		where TFilter : EmployeeFilterBase
		where TShort : class, IOrganisationElement, new()
	{
		public EmployeeTranslatorBase(SKDDatabaseService databaseService)
			: base(databaseService)
		{
		}

		protected abstract Guid? GetLinkUID(DataAccess.Employee employee);

		protected override Expression<Func<TTableType, bool>> IsInFilter(TFilter filter)
		{
			var result = base.IsInFilter(filter);
			if (filter.EmployeeUIDs.IsNotNullOrEmpty())
			{
				var employees = Context.Employees.Where(x => filter.EmployeeUIDs.Contains(x.UID)).ToList().Select(x => GetLinkUID(x));
				result = result.And(x => employees.Contains(x.UID));
			}
			return result;
		}
	}
}