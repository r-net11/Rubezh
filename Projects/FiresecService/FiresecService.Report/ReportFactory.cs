using StrazhAPI.Enums;
using StrazhAPI.SKD.ReportFilters;
using FiresecService.Report.Templates;
using System.Diagnostics;

namespace FiresecService.Report
{
	public sealed class ReportFactory
	{
		public static BaseReport CreateReport(ReportType reportType, SKDReportFilter filter)
		{
			BaseReport report = null;

			switch (reportType)
			{
				case ReportType.CardsReport:
					report = new CardsReport();
					break;
				case ReportType.DepartmentsReport:
					report = new DepartmentsReport();
					break;
				case ReportType.DisciplineReport:
					report = new DisciplineReport();
					break;
				case ReportType.DocumentsReport:
					report = new DocumentsReport();
					break;
				case ReportType.DoorsReport:
					report = new DoorsReport();
					break;
				case ReportType.EmployeeAccessReport:
					report = new EmployeeAccessReport();
					break;
				case ReportType.EmployeeDoorsReport:
					report = new EmployeeDoorsReport();
					break;
				case ReportType.EmployeeReport:
					report = new EmployeeReport();
					break;
				case ReportType.EmployeeRootReport:
					report = new EmployeeRootReport();
					break;
				case ReportType.EmployeeZonesReport:
					report = new EmployeeZonesReport();
					break;
				case ReportType.EventsReport:
					report = new EventsReport();
					break;
				case ReportType.PositionsReport:
					report = new PositionsReport();
					break;
				case ReportType.SchedulesReport:
					report = new SchedulesReport();
					break;
				case ReportType.WorkingTimeReport:
					report = new WorkingTimeReport();
					break;
			}

			Debug.Assert(report != null, "report != null");
			report.ApplyFilter(filter);

			return report;
		}
	}
}
