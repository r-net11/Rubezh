using Common;
using DevExpress.XtraReports.UI;
using FiresecService.Report.Templates;
using StrazhAPI.Enums;
using StrazhAPI.Extensions;
using StrazhAPI.SKD;
using StrazhAPI.SKD.ReportFilters;
using System;
using System.IO;

namespace FiresecService.Report.Export
{
	public class ReportExporter
	{
		private readonly ReportExportFilter _filter;

		public ReportExporter(ReportExportFilter filter)
		{
			if (filter == null || string.IsNullOrEmpty(filter.Path))
			{
				Logger.Error("Path is empty. Fill path to export report");
				throw new ArgumentNullException("filter");
			}

			_filter = filter;
		}

		public void Execute()
		{
			try
			{
				var skdReportFilter = InitializeFilterFromExport(_filter);
				var report = ReportFactory.CreateReport(_filter.ReportType, skdReportFilter);
				Directory.CreateDirectory(_filter.Path);

				ExportReport(report, _filter.ReportFormat, GetPath(report, _filter));
			}
			catch (Exception e)
			{
				Logger.Error(e);
				throw;
			}
		}

		protected SKDReportFilter InitializeFilterFromExport(ReportExportFilter filter)
		{
			var tmpFilter = filter.ReportFilter;

			var archive = tmpFilter as IReportFilterArchive;
			if (archive != null)
				archive.UseArchive = filter.IsShowArchive;

			var period = tmpFilter as IReportFilterPeriod;
			if (period != null)
			{
				period.PeriodType = filter.ReportPeriodType;
				SetDatePeriod(period, filter);
			}

			var reportFilter = tmpFilter as CardsReportFilter;
			if (reportFilter != null)
			{
				reportFilter.UseExpirationDate = filter.IsUseExpirationDate;
				if (filter.IsUseExpirationDate)
				{
					reportFilter.ExpirationType = filter.ReportEndDateType;
					SetExpirationDate(reportFilter, filter);
				}
			}

			var zonesReportFilter = tmpFilter as EmployeeZonesReportFilter;
			if (zonesReportFilter != null)
			{
				zonesReportFilter.UseCurrentDate = filter.IsUseDateTimeNow;
				zonesReportFilter.ReportDateTime = filter.IsUseDateTimeNow ? DateTime.Now : filter.StartDate;
			}

			return tmpFilter;
		}

		private static void SetExpirationDate(CardsReportFilter reportFilter, ReportExportFilter filter)
		{
			switch (filter.ReportEndDateType)
			{
				case EndDateType.Day:
					reportFilter.ExpirationDate = DateTime.Today.NextDay();
					break;

				case EndDateType.Week:
					reportFilter.ExpirationDate = DateTime.Today.NextWeek();
					break;

				case EndDateType.Month:
					reportFilter.ExpirationDate = DateTime.Today.NextMonth();
					break;

				case EndDateType.Arbitrary:
					reportFilter.ExpirationDate = filter.EndDate;
					break;
			}
		}

		private static void SetDatePeriod(IReportFilterPeriod filter, ReportExportFilter reportFilter)
		{
			switch (reportFilter.ReportPeriodType)
			{
				case ReportPeriodType.Month:
					filter.DateTimeFrom = DateTime.Today.PreviosStartMonth();
					filter.DateTimeTo = DateTime.Today.PreviosEndMonth();
					break;
				case ReportPeriodType.Week:
					filter.DateTimeFrom = DateTime.Today.PreviosWeekMonday();
					filter.DateTimeTo = DateTime.Today.PreviosWeekSunday();
					break;
				case ReportPeriodType.Day:
					filter.DateTimeFrom = DateTime.Today.PreviosStartDay();
					filter.DateTimeTo = DateTime.Today.PreviosEndDay();
					break;
				case ReportPeriodType.Arbitrary:
					filter.DateTimeFrom = reportFilter.StartDate;
					filter.DateTimeTo = reportFilter.EndDate;
					break;
			}
		}

		protected string GetPath(BaseReport report, ReportExportFilter filter)
		{
			return Path.Combine(filter.Path, GetReportName(report, filter));
		}

		private static string GetReportName(BaseReport report, ReportExportFilter filter)
		{
			var result = report.GetType().Name;

			if (filter.IsFilterNameInHeader)
				result += " (" + filter.ReportFilter.Name + ")";
			if (filter.IsUseDateInFileName)
			{
				var time = DateTime.Now;
				result += string.Format("_{0}-{1}-{2}_{3}_{4}_{5}", time.Day, time.Month, time.Year, time.Hour, time.Minute, time.Second);
			}

			return result + EnumHelper.GetEnumDescription(filter.ReportFormat).Replace("*", string.Empty);
		}

		private static void ExportReport(XtraReport report, ReportFormatEnum reportFormat, string path)
		{
			try
			{
				switch (reportFormat)
				{
					case ReportFormatEnum.Csv:
						report.ExportToCsv(path);
						break;
					case ReportFormatEnum.Html:
						report.ExportToHtml(path);
						break;
					case ReportFormatEnum.Mht:
						report.ExportToMht(path);
						break;
					case ReportFormatEnum.Pdf:
						report.ExportToPdf(path);
						break;
					case ReportFormatEnum.Png:
						report.ExportToImage(path);
						break;
					case ReportFormatEnum.Rtf:
						report.ExportToRtf(path);
						break;
					case ReportFormatEnum.Txt:
						report.ExportToText(path);
						break;
					case ReportFormatEnum.Xls:
						report.ExportToXls(path);
						break;
					case ReportFormatEnum.Xlsx:
						report.ExportToXlsx(path);
						break;
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "ExportReport");
				throw;
			}
		}
	}
}
