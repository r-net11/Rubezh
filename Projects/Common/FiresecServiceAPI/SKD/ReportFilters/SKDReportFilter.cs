using System;
using System.Runtime.Serialization;
using StrazhAPI.Enums;

namespace StrazhAPI.SKD.ReportFilters
{
	[DataContract]
	[KnownType(typeof(SKDReportFilter))]
	[KnownType(typeof(CardsReportFilter))]
	[KnownType(typeof(DepartmentsReportFilter))]
	[KnownType(typeof(DisciplineReportFilter))]
	[KnownType(typeof(DocumentsReportFilter))]
	[KnownType(typeof(DoorsReportFilter))]
	[KnownType(typeof(EmployeeReportFilter))]
	[KnownType(typeof(EmployeeRootReportFilter))]
	[KnownType(typeof(EmployeeZonesReportFilter))]
	[KnownType(typeof(EventsReportFilter))]
	[KnownType(typeof(PositionsReportFilter))]
	[KnownType(typeof(SchedulesReportFilter))]
	[KnownType(typeof(WorkingTimeReportFilter))]
	public class SKDReportFilter
	{
		public SKDReportFilter()
		{
			Name = DefaultFilterName;

			PrintFilterName = true;
			PrintFilterNameInHeader = false;
			PrintPeriod = true;
			PrintDate = true;
			PrintUser = true;

			SortAscending = true;

			if (this is IReportFilterPeriod)
			{
				var periodFilter = (IReportFilterPeriod)this;
				periodFilter.DateTimeFrom = DateTime.Today;
				periodFilter.DateTimeTo = DateTime.Today.AddDays(1).AddSeconds(-1);
				periodFilter.PeriodType = ReportPeriodType.Day;
			}
			UserUID = Guid.Empty;
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string User { get; set; }

		[DataMember]
		public DateTime Timestamp { get; set; }

		[DataMember]
		public Guid UserUID { get; set; }

		[DataMember]
		public ReportType ReportType { get; set; }

		[DataMember]
		public string SortColumn { get; set; }

		[DataMember]
		public bool SortAscending { get; set; }

		[DataMember]
		public bool PrintFilterName { get; set; }

		[DataMember]
		public bool PrintFilterNameInHeader { get; set; }

		[DataMember]
		public bool PrintPeriod { get; set; }

		[DataMember]
		public bool PrintDate { get; set; }

		[DataMember]
		public bool PrintUser { get; set; }

		public bool IsDefault { get { return Name == DefaultFilterName; } }

        private string DefaultFilterName { get { return Resources.Language.SKD.ReportFilters.SKDReportFilter.DefaultFilterName; } }

		public override string ToString()
		{
			return Name;
		}
	}
}