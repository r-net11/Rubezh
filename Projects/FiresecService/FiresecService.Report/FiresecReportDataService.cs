using System;
using System.Collections.Generic;
using System.Data;
using FiresecService.Report.Reports;

namespace FiresecService.Report
{
	class FiresecReportDataService : IReportDataService
	{
		public List<CardData> GetCardsReport()
		{
			using (var dataProvider = new DataProvider())
			{
				return cardsReport.Value.CreateDataSet(dataProvider);
			}
		}

		public List<DepartmentData> GetDepartmentsReport()
		{
			using (var dataProvider = new DataProvider())
			{
				return departmentsReport.Value.CreateDataSet(dataProvider);
			}
		}

		public List<DeviceData> GetDevicesReport()
		{
			using (var dataProvider = new DataProvider())
			{
				return devicesReport.Value.CreateDataSet(dataProvider);
			}
		}

		public List<DisciplineData> GetDisciplineReport()
		{
			using (var dataProvider = new DataProvider())
			{
				return disciplineReport.Value.CreateDataSet(dataProvider);
			}
		}

		public List<DocumentData> GetDocumentsReport()
		{
			using (var dataProvider = new DataProvider())
			{
				return documentsReport.Value.CreateDataSet(dataProvider);
			}
		}

		public List<DoorData> GetDoorsReport()
		{
			using (var dataProvider = new DataProvider())
			{
				return doorsReport.Value.CreateDataSet(dataProvider);
			}
		}

		public List<EmployeeAccessData> GetEmployeeAccessReport()
		{
			using (var dataProvider = new DataProvider())
			{
				return employeeAccessReport.Value.CreateDataSet(dataProvider);
			}
		}

		public List<EmployeeDoorsData> GetEmployeeDoorsReport()
		{
			using (var dataProvider = new DataProvider())
			{
				return employeeDoorsReport.Value.CreateDataSet(dataProvider);
			}
		}

		public EmployeeReportData GetEmployeeReport()
		{
			using (var dataProvider = new DataProvider())
			{
				return employeeReport.Value.CreateDataSet(dataProvider);
			}
		}

		public EmployeeRootReportData GetEmployeeRootReport()
		{
			using (var dataProvider = new DataProvider())
			{
				return employeeRootReport.Value.CreateDataSet(dataProvider);
			}
		}

		public List<EmployeeZonesData> GetEmployeeZonesReport()
		{
			using (var dataProvider = new DataProvider())
			{
				return employeeZonesReport.Value.CreateDataSet(dataProvider);
			}
		}

		public DataSet GetEmptyReport()
		{
			using (var dataProvider = new DataProvider())
			{
				return emptyReport.Value.CreateDataSet(dataProvider);
			}
		}

		public List<EventsData> GetEventsReport()
		{
			using (var dataProvider = new DataProvider())
			{
				return eventsReport.Value.CreateDataSet(dataProvider);
			}
		}

		public List<PositionsData> GetPositionsReport()
		{
			using (var dataProvider = new DataProvider())
			{
				return positionsReport.Value.CreateDataSet(dataProvider);
			}
		}

		public List<ReflectionData> GetReflectionReport()
		{
			using (var dataProvider = new DataProvider())
			{
				return reflectionReport.Value.CreateDataSet(dataProvider);
			}
		}

		public List<SchedulesData> GetSchedulesReport()
		{
			using (var dataProvider = new DataProvider())
			{
				return schedulesReport.Value.CreateDataSet(dataProvider);
			}
		}

		public List<WorkingTimeData> GetWorkingTimeReport()
		{
			using (var dataProvider = new DataProvider())
			{
				return workingTimeReport.Value.CreateDataSet(dataProvider);
			}
		}

		#region Fields
		private static readonly Lazy<CardsReport> cardsReport = new Lazy<CardsReport>();
		private static readonly Lazy<DepartmentsReport> departmentsReport = new Lazy<DepartmentsReport>();
		private static readonly Lazy<DevicesReport> devicesReport = new Lazy<DevicesReport>();
		private static readonly Lazy<DisciplineReport> disciplineReport = new Lazy<DisciplineReport>();
		private static readonly Lazy<DocumentsReport> documentsReport = new Lazy<DocumentsReport>();
		private static readonly Lazy<DoorsReport> doorsReport = new Lazy<DoorsReport>();
		private static readonly Lazy<EmployeeAccessReport> employeeAccessReport = new Lazy<EmployeeAccessReport>();
		private static readonly Lazy<EmployeeDoorsReport> employeeDoorsReport = new Lazy<EmployeeDoorsReport>();
		private static readonly Lazy<EmployeeReport> employeeReport = new Lazy<EmployeeReport>();
		private static readonly Lazy<EmployeeRootReport> employeeRootReport = new Lazy<EmployeeRootReport>();
		private static readonly Lazy<EmployeeZonesReport> employeeZonesReport = new Lazy<EmployeeZonesReport>();
		private static readonly Lazy<EmptyReport> emptyReport = new Lazy<EmptyReport>();
		private static readonly Lazy<EventsReport> eventsReport = new Lazy<EventsReport>();
		private static readonly Lazy<PositionsReport> positionsReport = new Lazy<PositionsReport>();
		private static readonly Lazy<ReflectionReport> reflectionReport = new Lazy<ReflectionReport>();
		private static readonly Lazy<SchedulesReport> schedulesReport = new Lazy<SchedulesReport>();
		private static readonly Lazy<WorkingTimeReport> workingTimeReport = new Lazy<WorkingTimeReport>();

		#endregion
	}
}
