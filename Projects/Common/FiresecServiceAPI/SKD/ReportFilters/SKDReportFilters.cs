using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.SKD.ReportFilters
{
	[DataContract]
	[XmlInclude(typeof(ReportFilter401))]
	[XmlInclude(typeof(ReportFilter402))]
	[XmlInclude(typeof(ReportFilter411))]
	[XmlInclude(typeof(ReportFilter412))]
	[XmlInclude(typeof(ReportFilter413))]
	[XmlInclude(typeof(ReportFilter415))]
	[XmlInclude(typeof(ReportFilter416))]
	[XmlInclude(typeof(ReportFilter417))]
	[XmlInclude(typeof(ReportFilter418))]
	[XmlInclude(typeof(ReportFilter421))]
	[XmlInclude(typeof(ReportFilter422))]
	[XmlInclude(typeof(ReportFilter423))]
	[XmlInclude(typeof(ReportFilter424))]
	[XmlInclude(typeof(ReportFilter431))]
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
