using System;
using System.ComponentModel.Composition;
using DevExpress.XtraReports.Service.Extensions;
using DevExpress.XtraReports.UI;
using RubezhService.Report.Templates;

namespace RubezhService.Report
{
	[Export(typeof(IReportResolver))]
	public class ReportResolver : IReportResolver
	{
		private const string ReportNameTemplate = @"RubezhService.Report.Templates.{0}, RubezhService.Report";
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
