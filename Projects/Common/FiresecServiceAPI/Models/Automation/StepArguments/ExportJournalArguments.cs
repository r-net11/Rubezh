using System;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using FiresecAPI.Enums;
using FiresecAPI.SKD.ReportFilters;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ExportJournalArguments
	{
		public ExportJournalArguments()
		{
			IsExportJournalArgument = new Argument();
			IsExportPassJournalArgument = new Argument();
			MinDateArgument = new Argument();
			MaxDateArgument = new Argument();
			PathArgument = new Argument();
		}

		[DataMember]
		public Argument IsExportJournalArgument { get; set; }

		[DataMember]
		public Argument IsExportPassJournalArgument { get; set; }

		[DataMember]
		public Argument MinDateArgument { get; set; }

		[DataMember]
		public Argument MaxDateArgument { get; set; }

		[DataMember]
		public Argument PathArgument { get; set; }
	}

	[DataContract]
	public class ExportOrganisationArguments
	{
		public ExportOrganisationArguments()
		{
			Organisation = new Argument();
			IsWithDeleted = new Argument();
			PathArgument = new Argument();
		}

		[DataMember]
		public Argument Organisation { get; set; }

		[DataMember]
		public Argument IsWithDeleted { get; set; }

		[DataMember]
		public Argument PathArgument { get; set; }
	}

	[DataContract]
	public class ExportConfigurationArguments
	{
		public ExportConfigurationArguments()
		{
			IsExportDevices = new Argument();
			IsExportDoors = new Argument();
			IsExportZones = new Argument();
			PathArgument = new Argument();
		}

		[DataMember]
		public Argument IsExportDevices { get; set; }

		[DataMember]
		public Argument IsExportDoors { get; set; }

		[DataMember]
		public Argument IsExportZones { get; set; }

		[DataMember]
		public Argument PathArgument { get; set; }
	}

	[DataContract]
	public class ImportOrganisationArguments
	{
		public ImportOrganisationArguments()
		{
			IsWithDeleted = new Argument();
			PathArgument = new Argument();
		}

		[DataMember]
		public Argument IsWithDeleted { get; set; }

		[DataMember]
		public Argument PathArgument { get; set; }
	}

	[DataContract]
	[XmlInclude(typeof(SKDReportFilter))]
	[XmlInclude(typeof(DoorsReportFilter))]
	[XmlInclude(typeof(CardsReportFilter))]
	[XmlInclude(typeof(DepartmentsReportFilter))]
	[XmlInclude(typeof(DisciplineReportFilter))]
	[XmlInclude(typeof(DocumentsReportFilter))]
	[XmlInclude(typeof(EmployeeReportFilter))]
	[XmlInclude(typeof(EmployeeRootReportFilter))]
	[XmlInclude(typeof(EmployeeZonesReportFilter))]
	[XmlInclude(typeof(EventsReportFilter))]
	[XmlInclude(typeof(PositionsReportFilter))]
	[XmlInclude(typeof(SchedulesReportFilter))]
	[XmlInclude(typeof(WorkingTimeReportFilter))]
	[XmlInclude(typeof(EmployeeAccessReportFilter))]
	public class ExportReportArguments
	{
		public ExportReportArguments()
		{
			FilePath = new Argument();
			StartDate = new Argument();
			EndDate = new Argument();
		}

		[DataMember]
		public Argument FilePath { get; set; }
		[DataMember]
		public Argument StartDate { get; set; }
		[DataMember]
		public Argument EndDate { get; set; }

		[DataMember]
		public SKDReportFilter ReportFilter { get; set; }
		[DataMember]
		public ReportType ReportType { get; set; }
		[DataMember]
		public ReportPeriodType ReportPeriodType { get; set; }
		[DataMember]
		public ReportFormatEnum ReportFormat { get; set; }
		[DataMember]
		public EndDateType ReportEndDateType { get; set; }
		[DataMember]
		public bool IsFilterNameInHeader { get; set; }
		[DataMember]
		public bool IsUseArchive { get; set; }
		[DataMember]
		public bool IsUseExpirationDate { get; set; }
		[DataMember]
		public bool IsUseDateTimeNow { get; set; }
	}
}