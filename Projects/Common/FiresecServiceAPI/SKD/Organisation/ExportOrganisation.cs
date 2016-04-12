using System;
using FiresecAPI.Enums;
using FiresecAPI.SKD.ReportFilters;

namespace FiresecAPI.SKD
{
	public class ExportOrganisation : IExportItem
	{
		public Guid UID { get; set; }

		public string ExternalKey { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public string Phone { get; set; }

		public bool IsDeleted { get; set; }

		public DateTime RemovalDate { get; set; }

		public Guid ChiefUID { get; set; }

		public string ChiefExternalKey { get; set; }

		public Guid HRChiefUID { get; set; }

		public string HRChiefExternalKey { get; set; }
	}

	public class ExportEmployee : IExportItem
	{
		public Guid UID { get; set; }

		public string ExternalKey { get; set; }

		public string FirstName { get; set; }

		public string SecondName { get; set; }

		public string LastName { get; set; }

		public string DocumentNumber { get; set; }

		public DateTime? BirthDate { get; set; }

		public string BirthPlace { get; set; }

		public DateTime? DocumentGivenDate { get; set; }

		public string DocumentGivenBy { get; set; }

		public DateTime? DocumentValidTo { get; set; }

		public string DocumentDepartmentCode { get; set; }

		public string Citizenship { get; set; }

		public string Description { get; set; }

		public int? Gender { get; set; }

		public int Type { get; set; }

		public bool IsDeleted { get; set; }

		public DateTime RemovalDate { get; set; }

		public DateTime LastEmployeeDayUpdate { get; set; }

		public DateTime ScheduleStartDate { get; set; }

		public Guid OrganisationUID { get; set; }

		public string OrganisationExternalKey { get; set; }

		public Guid PositionUID { get; set; }

		public string PositionExternalKey { get; set; }

		public Guid DepartmentUID { get; set; }

		public string DepartmentExternalKey { get; set; }

		public Guid EscrortUID { get; set; }

		public string EscortExternalKey { get; set; }
	}

	public class ExportDepartment : IExportItem
	{
		public Guid UID { get; set; }

		public string ExternalKey { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public string Phone { get; set; }

		public bool IsDeleted { get; set; }

		public DateTime RemovalDate { get; set; }

		public Guid OrganisationUID { get; set; }

		public string OrganisationExternalKey { get; set; }

		public Guid ParentDepartmentUID { get; set; }

		public string ParentDepartmentExternalKey { get; set; }

		public Guid ContactEmployeeUID { get; set; }

		public string ContactEmployeeExternalKey { get; set; }

		public Guid AttendantUID { get; set; }

		public string AttendantExternalKey { get; set; }

		public Guid ChiefUID { get; set; }

		public string ChiefExternalKey { get; set; }
	}

	public class ExportPosition : IExportItem
	{
		public Guid UID { get; set; }

		public string ExternalKey { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public bool IsDeleted { get; set; }

		public DateTime RemovalDate { get; set; }

		public Guid OrganisationUID { get; set; }

		public string OrganisationExternalKey { get; set; }
	}

	public class ExportCard
	{
		public Guid UID { get; set; }

		public string ExternalKey { get; set; }

		public int Number { get; set; }

		public int CardType { get; set; }

		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }

		public bool IsInStopList { get; set; }

		public string StopReason { get; set; }

		public string Password { get; set; }

		public int UserTime { get; set; }

		public Guid OrganisationUID { get; set; }

		public string OrganisationExternalKey { get; set; }

		public Guid EmployeeUID { get; set; }

		public string EmployeeExternalKey { get; set; }
	}

	public class ExportFilter
	{
		public Guid OrganisationUID { get; set; }

		public string OrganisationName { get; set; }

		public bool IsWithDeleted { get; set; }

		public string Path { get; set; }
	}

	public class JournalExportFilter
	{
		public DateTime MinDate { get; set; }

		public DateTime MaxDate { get; set; }

		public string Path { get; set; }

		public bool IsExportJournal { get; set; }

		public bool IsExportPassJournal { get; set; }
	}

	public class ConfigurationExportFilter
	{
		public string Path { get; set; }

		public bool IsExportDevices { get; set; }

		public bool IsExportZones { get; set; }

		public bool IsExportDoors { get; set; }
	}

	public class ReportExportFilter
	{
		public string Path { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public SKDReportFilter ReportFilter { get; set; }
		public ReportType ReportType { get; set; }
		public ReportPeriodType ReportPeriodType { get; set; }
		public ReportFormatEnum ReportFormat { get; set; }
		public EndDateType ReportEndDateType { get; set; }
		public bool IsFilterNameInHeader { get; set; }
		public bool IsShowArchive { get; set; }
		public bool IsUseExpirationDate { get; set; }
		public bool IsUseDateTimeNow { get; set; }
		public bool IsUseDateInFileName { get; set; }
	}

	public class ImportFilter
	{
		public bool IsWithDeleted { get; set; }

		public string Path { get; set; }
	}

	public interface IExportItem
	{
		Guid UID { get; set; }

		string ExternalKey { get; set; }

		bool IsDeleted { get; set; }

		DateTime RemovalDate { get; set; }
	}
}