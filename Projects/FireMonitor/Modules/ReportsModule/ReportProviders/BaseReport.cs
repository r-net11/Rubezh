using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using CodeReason.Reports;
using System.Windows;
using Infrastructure.Common;
using System.IO;
using System.Windows.Resources;
using System.Diagnostics;
using FiresecAPI.Models;
using FiresecAPI;

namespace ReportsModule.ReportProviders
{
	public abstract class BaseReport
	{
		private const string TemplateFormat = "/ReportTemplates/{0}Template.xaml";

		public BaseReport(ReportType reportType)
		{
			ReportType = reportType;
		}

		public ReportType ReportType { get; private set; }
		public string Title
		{
			get { return ReportType.ToDescription(); }
		}
		public abstract bool IsFilterable { get; }

		public DocumentPaginator GenerateReport()
		{
			DateTime dt = DateTime.Now;
			ReportDocument reportDocument = new ReportDocument();
			reportDocument.XamlData = GetXaml();
			ReportData reportData = GetData();
			ReportPaginator reportPaginator = new ReportPaginator(reportDocument, reportData);
			Debug.WriteLine("Build report: {0}", DateTime.Now - dt);
			return reportPaginator;
		}

		public abstract ReportData GetData();
		public virtual void Filter()
		{
		}

		private string GetXaml()
		{
			StreamResourceInfo info = Application.GetResourceStream(ResourceHelper.ComposeResourceUri(GetType().Assembly, string.Format(TemplateFormat, GetType().Name)));
			using (var reader = new StreamReader(info.Stream))
				return reader.ReadToEnd();
		}
	}
}
