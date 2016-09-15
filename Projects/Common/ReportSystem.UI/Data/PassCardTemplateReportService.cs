using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraReports.UI;
using ReportSystem.Api.DTO;
using SKDModule.Reports;
using DataColumn = System.Data.DataColumn;

namespace ReportSystem.UI.Data
{
	/// <summary>
	/// Сервис, формирующий источник данных для отчётов дизайнера пропуска.
	/// </summary>
	public class PassCardTemplateReportService
	{
		/// <summary>
		/// Добавляет колонки к источнику данных.
		/// </summary>
		/// <param name="columns">Список колонок, которые необходимо добавить.</param>
		/// <param name="source">Объект источника данных, в который должны добавится колонки.</param>
		private static void AddAdditionalColumnsToSource(IEnumerable<DataColumn> columns, PassCardTemplateSource source)
		{
			source.Tables[0].Columns.AddRange(columns.ToArray());
		}

		public PassCardTemplateLocalizeDataSource GetEmptyDataSource(IEnumerable<DataColumn> columns = null)
		{
			var source = new PassCardTemplateLocalizeDataSource();

			if (columns != null)
				AddAdditionalColumnsToSource(columns, source);

			return source;
		}

		/// <summary>
		/// Метод, который разбирает DTO-отчёты и формирует из них объекты DevExpress отчётов.
		/// </summary>
		/// <param name="serverReports">Коллекция DTO-отчётов</param>
		/// <returns>Коллекция объектов сфотмированных DevExpress отчётов</returns>
		public IEnumerable<XtraReport> GetReportsFromDTO(IEnumerable<ReportDTO> serverReports)
		{
			var resultXtraReports = new List<XtraReport>();
			foreach (var xReport in serverReports)
			{
				var ds = new PassCardTemplateSource();

				var additionalColumns = xReport.Data.AdditionalColumns.ToList().Select(x => x.GraphicValue != null
					? new DataColumn(x.Name, typeof(byte[]))
					: new DataColumn(x.Name, typeof(string)));

				AddAdditionalColumnsToSource(additionalColumns, ds);

				var row = ds.Employee.NewEmployeeRow();
				row.FirstName = xReport.Data.FirstName;
				row.MiddleName = xReport.Data.MiddleName;
				row.LastName = xReport.Data.LastName;
				row.Image = xReport.BackgroundImage;

				foreach (var column in xReport.Data.AdditionalColumns)
				{
					if (column.GraphicValue != null)
						row[column.Name] = column.GraphicValue;
					else
						row[column.Name] = column.TextValue;
				}

				ds.Employee.AddEmployeeRow(row);

				var report = xReport.Report.ToXtraReport(xReport.BackgroundImage);
				report.DataSource = ds;
				report.DataMember = ds.Tables[0].TableName;

				resultXtraReports.Add(report);
			}

			return resultXtraReports;
		}
	}
}
