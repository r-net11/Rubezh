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
		private const string ReportNameTemplate = @"FiresecService.Report.Templates.{0}, FiresecService.Report";
		public ReportResolver()
		{
		}

		#region IReportResolver Members

		public XtraReport Resolve(string reportName, bool getParameters)
		{
			var name = string.Format(ReportNameTemplate, reportName);
			var type = Type.GetType(name);
			if (type == null)
				return null;
			return (XtraReport)Activator.CreateInstance(type);
		}

		#endregion
	}
}
