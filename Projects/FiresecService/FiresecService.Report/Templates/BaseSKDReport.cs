using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.SKD.ReportFilters;
using SKDDriver;
using FiresecAPI;
using System.Data.SqlClient;
using System.Data;
using DevExpress.XtraReports.UI;
using System.Drawing;

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
#if DEBUG
            PrintFilter();
#endif
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

        protected void FillTestData(int count = 20)
        {
            if (!DataSet.Tables.Contains(DataMember))
                throw new ApplicationException();
            var dt = DataSet.Tables[DataMember];
			FillTestData(dt, count);
        }
		protected void FillTestData(DataTable table, int count)
		{
			for (int i = 0; i < count; i++)
			{
				var row = table.NewRow();
				foreach (DataColumn column in table.Columns)
				{
					if (column.DataType == typeof(string))
						row[column] = string.Format("{0} {1}", column.ColumnName, i);
					else if (column.DataType == typeof(int) || column.DataType == typeof(long))
						row[column] = i;
					else if (column.DataType == typeof(double) || column.DataType == typeof(decimal))
						row[column] = i;
					else if (column.DataType == typeof(DateTime))
						row[column] = DateTime.Today.AddDays(-i);
					else if (column.DataType == typeof(TimeSpan))
						row[column] = new TimeSpan(i, i + 1, i + 2);
					else if (column.DataType == typeof(bool))
						row[column] = i % 2 == 0;
					else if (column.DataType == typeof(Guid))
						row[column] = Guid.NewGuid();
				}
				table.Rows.Add(row);
			}
		}
        protected void PrintFilter()
        {
            var footer = new ReportFooterBand()
            {
                Borders = DevExpress.XtraPrinting.BorderSide.All,
                BorderWidth = 3,
                KeepTogether = true,
            };
            var label = new XRLabel()
            {
                CanGrow = true,
                CanShrink = true,
                Multiline = true,
                LocationF = new PointF(0, 20),
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft,
                WordWrap = true,
                Text = BuildFilterString(),
            };
            footer.Controls.Add(label);
            Bands.Add(footer);
            label.WidthF = footer.RightF;
        }
        private string BuildFilterString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("ФИЛЬТР:");
            foreach (var property in Filter.GetType().GetProperties().OrderBy(prop=>prop.Name))
            {
                var propType = property.PropertyType;
                var value = property.GetValue(Filter, new object[0]);
                if (propType == typeof(List<Guid>))
                    sb.AppendFormat("{0} = {{{1}}}\r\n", property.Name, value == null ? "NULL" : string.Join(",", ((List<Guid>)value).ToArray()));
                else
                    sb.AppendFormat("{0} = '{1}'\r\n", property.Name, value);
            }
            return sb.ToString();
        }
    }
}
