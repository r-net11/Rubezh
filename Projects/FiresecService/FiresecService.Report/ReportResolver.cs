using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraReports.Service.Extensions;
using System.ComponentModel.Composition;
using DevExpress.XtraReports.UI;

namespace FiresecService.Report
{
	[Export(typeof(IReportResolver))]
	public class ReportResolver : IReportResolver
	{
		public ReportResolver()
		{
		}

		#region IReportResolver Members

		public XtraReport Resolve(string reportName, bool getParameters)
		{
			var type = Type.GetType(reportName);
			return (XtraReport)Activator.CreateInstance(type);
		}

		#endregion
	}
}
