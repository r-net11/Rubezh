using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.SKD.ReportFilters;
using SKDDriver;
using FiresecAPI;
using System.Data.SqlClient;
using System.Data;

namespace FiresecService.Report.Templates
{
	public class BaseSKDReport : BaseReport
	{
		protected DataSet DataSet { get; private set; }

		protected override void DataSourceRequered()
		{
			DataSet = CreateDataSet();
			var query = BuildQuery();
			if (query.Length > 0)
				using (var connection = new SqlConnection(SKDDatabaseService.ConnectionString))
				{
					var adapter = new SqlDataAdapter(query.ToString(), connection);
					AddTableMapping(adapter, DataSet);
					adapter.Fill(DataSet);
				}
			UpdateDataSource();
			DataSource = DataSet;
		}

		protected T GetFilter<T>()
			where T : SKDReportFilter
		{
			return (T)Filter ?? Activator.CreateInstance<T>();
		}

		protected virtual void AddTableMapping(IDataAdapter adapter, DataSet dataSet)
		{
			adapter.TableMappings.Add("Table", dataSet.Tables[0].TableName);
		}
		protected virtual StringBuilder BuildQuery()
		{
			var sb = new StringBuilder();
			sb.Append(BuildSelectRoutine());
			if (sb.Length == 0)
				return sb;
			var sql = BuildWhereRouting();
			if (sql.Length > 0)
				sb.AppendFormat("{0}{1}", SqlBuilder.WHERE, sql);
			sql = BuildOrderRouting();
			if (sql.Length > 0)
				sb.AppendFormat("{0}{1}", SqlBuilder.ORDERBY, sql);
			return sb;
		}
		protected virtual string BuildSelectRoutine()
		{
			return string.Empty;
		}
		protected virtual string BuildWhereRouting()
		{
			return string.Empty;
		}
		protected virtual string BuildOrderRouting()
		{
			var column = Filter.SortColumn;
			if (string.IsNullOrEmpty(column))
				column = DataSet.Tables[0].Columns[0].ColumnName;
			var asc = Filter.SortAscending ? SqlBuilder.ASC : SqlBuilder.DESC;
			return string.Format("{0} {1}", column, asc);
		}

		protected virtual DataSet CreateDataSet()
		{
			return new DataSet();
		}
		protected virtual void UpdateDataSource()
		{
		}
	}
}
