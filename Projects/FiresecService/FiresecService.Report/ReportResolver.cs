using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraReports.Service.Extensions;
using System.ComponentModel.Composition;
using DevExpress.XtraReports.UI;
using FiresecService.Report.Templates;

namespace FiresecService.Report
{
	[Export(typeof(IReportResolver))]
	public class ReportResolver : IReportResolver
	{
		private const string ReportNameTemplate = @"FiresecService.Report.Templates.{0}, FiresecService.Report";
		private const string ProviderSuffix = "Provider";
		public ReportResolver()
		{
		}

		private string GetReportName(string name)
		{
			return name.EndsWith(ProviderSuffix) ? name.Substring(0, name.Length - ProviderSuffix.Length) : name;
		}

		#region IReportResolver Members

		public XtraReport Resolve(string reportName, bool getParameters)
		{
			if (getParameters)
				return new EmptyReport();
			var name = string.Format(ReportNameTemplate, GetReportName(reportName));
			var type = Type.GetType(name);
			if (type == null)
				return new EmptyReport();
			return (XtraReport)Activator.CreateInstance(type);
		}

		#endregion
	}
}
