using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.SKD.ReportFilters
{
	[DataContract]
	[XmlInclude(typeof(ReportFilter415))]
    [XmlInclude(typeof(ReportFilter416))]
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
