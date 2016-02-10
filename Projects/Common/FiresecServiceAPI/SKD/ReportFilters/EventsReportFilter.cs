using FiresecAPI.Enums;
using FiresecAPI.Journal;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD.ReportFilters
{
	[DataContract]
	[KnownType(typeof(EventsReportFilter))]
	public class EventsReportFilter : SKDReportFilter, IReportFilterPeriod, IReportFilterEmployeeAndVisitor, IReportFilterUser
	{
		public EventsReportFilter()
		{
			JournalEventSubsystemTypes = new List<JournalSubsystemType>();
			JournalOjbectSubsystemTypes = new List<JournalSubsystemType>();
			JournalEventNameTypes = new List<JournalEventNameType>();
			JournalObjectTypes = new List<JournalObjectType>();
			ObjectUIDs = new List<Guid>();
			IsEmployee = true;
			ReportType = ReportType.EventsReport;
		}

		#region IReportFilterPeriod Members

		[DataMember]
		public ReportPeriodType PeriodType { get; set; }

		[DataMember]
		public DateTime DateTimeFrom { get; set; }

		[DataMember]
		public DateTime DateTimeTo { get; set; }

		#endregion IReportFilterPeriod Members

		#region IReportFilterEmployee Members

		[DataMember]
		public List<Guid> Employees { get; set; }

		[DataMember]
		public bool IsSearch { get; set; }

		[DataMember]
		public string LastName { get; set; }

		[DataMember]
		public string FirstName { get; set; }

		[DataMember]
		public string SecondName { get; set; }

		#endregion IReportFilterEmployee Members

		#region IReportFilterEmployeeAndVisitor Members

		[DataMember]
		public bool IsEmployee { get; set; }

		#endregion IReportFilterEmployeeAndVisitor Members

		#region IReportFilterUser Members

		[DataMember]
		public List<string> Users { get; set; }

		#endregion IReportFilterUser Members

		[DataMember]
		public List<JournalSubsystemType> JournalEventSubsystemTypes { get; set; }

		[DataMember]
		public List<JournalEventNameType> JournalEventNameTypes { get; set; }

		[DataMember]
		public List<JournalSubsystemType> JournalOjbectSubsystemTypes { get; set; }

		[DataMember]
		public List<JournalObjectType> JournalObjectTypes { get; set; }

		[DataMember]
		public List<Guid> ObjectUIDs { get; set; }
	}
}