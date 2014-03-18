using System;
using System.Linq.Expressions;
using FiresecAPI;
using LinqKit;

namespace SKDDriver
{
	public class AdditionalColumnTranslator : TranslatorBase<DataAccess.AdditionalColumn, AdditionalColumn, AdditionalColumnFilter>
	{
		public AdditionalColumnTranslator(DataAccess.SKUDDataContext context)
			: base(context)
		{

		}

		protected override AdditionalColumn Translate(DataAccess.AdditionalColumn tableItem)
		{
			var result = base.Translate(tableItem);
			result.EmployeeUID = tableItem.EmployeeUID;
			result.AdditionalColumnTypeUID = tableItem.AdditionalColumnTypeUID;
			result.GraphicsData = tableItem.GraphicsData.ToArray();
			result.TextData = tableItem.TextData;
			return result;
		}

		protected override void TranslateBack(DataAccess.AdditionalColumn tableItem, AdditionalColumn apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.EmployeeUID = apiItem.EmployeeUID;
			tableItem.AdditionalColumnTypeUID = apiItem.AdditionalColumnTypeUID;
			tableItem.GraphicsData = apiItem.GraphicsData;
			tableItem.TextData = apiItem.TextData;
		}

		protected override Expression<Func<DataAccess.AdditionalColumn, bool>> IsInFilter(AdditionalColumnFilter filter)
		{
			var result = base.IsInFilter(filter);
			var employeeUIDs = filter.EmployeeUIDs;
			if (employeeUIDs != null && employeeUIDs.Count != 0)
				result = result.And(e => e.EmployeeUID != null && employeeUIDs.Contains(e.EmployeeUID.Value));
			var columnTypeUIDs = filter.ColumnTypeUIDs;
			if (columnTypeUIDs != null && columnTypeUIDs.Count != 0)
				result = result.And(e => e.AdditionalColumnTypeUID != null && columnTypeUIDs.Contains(e.AdditionalColumnTypeUID.Value));
			return result;
		}

	}
}


