using System;
using System.Collections.Generic;
using System.Data;

namespace RubezhService.Report
{
	public static class DataSetHelper
	{
		public static DataTable EnumerableToDataTable<T>(IEnumerable<T> list)
		{
			var elementType = typeof(T);
			using (var dataTable = new DataTable())
			{
				var properties = elementType.GetProperties();
				foreach (var propertyInfo in properties)
				{
					var propertyType = propertyInfo.PropertyType;
					var ColType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
					dataTable.Columns.Add(propertyInfo.Name, ColType);
				}
				foreach (var item in list)
				{
					var row = dataTable.NewRow();
					foreach (var propertyInfo in properties)
					{
						row[propertyInfo.Name] = propertyInfo.GetValue(item, null) ?? DBNull.Value;
					}
					dataTable.Rows.Add(row);
				}
				return dataTable;
			}
		}

		public static DataSet EnumerableToDataSet<T>(IEnumerable<T> list)
		{
			using (var dataSet = new DataSet())
			{
				dataSet.Tables.Add(EnumerableToDataTable(list));
				return dataSet;
			}
		}

	}
}
