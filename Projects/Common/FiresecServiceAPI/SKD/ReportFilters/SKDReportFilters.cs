using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace StrazhAPI.SKD.ReportFilters
{
	[DataContract]
	[XmlInclude(typeof(EventsReportFilter))]
	[XmlInclude(typeof(EmployeeRootReportFilter))]
	[XmlInclude(typeof(CardsReportFilter))]
	[XmlInclude(typeof(EmployeeAccessReportFilter))]
	[XmlInclude(typeof(EmployeeDoorsReportFilter))]
	[XmlInclude(typeof(DepartmentsReportFilter))]
	[XmlInclude(typeof(PositionsReportFilter))]
	[XmlInclude(typeof(EmployeeZonesReportFilter))]
	[XmlInclude(typeof(EmployeeReportFilter))]
	[XmlInclude(typeof(DisciplineReportFilter))]
	[XmlInclude(typeof(SchedulesReportFilter))]
	[XmlInclude(typeof(DocumentsReportFilter))]
	[XmlInclude(typeof(WorkingTimeReportFilter))]
	[XmlInclude(typeof(DoorsReportFilter))]
	public class SKDReportFilters
	{
		public SKDReportFilters()
		{
			Filters = new List<SKDReportFilter>();
		}

		[DataMember]
		public List<SKDReportFilter> Filters { get; set; }
	}
}