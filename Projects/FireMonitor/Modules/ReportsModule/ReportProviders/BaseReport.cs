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

namespace ReportsModule.ReportProviders
{
	internal abstract class BaseReport
	{
		private const string TemplateFormat = "/ReportTemplates/{0}Template.xaml";

		public DocumentPaginator GenerateReport()
		{
			DateTime dt1 = DateTime.Now;
			ReportDocument reportDocument = new ReportDocument();
			reportDocument.XamlData = GetXaml();
			ReportData reportData = GetData();
			ReportPaginator reportPaginator = new ReportPaginator(reportDocument, reportData);
			DateTime dt2 = DateTime.Now;
			Console.WriteLine("Build report: {0}", dt2 - dt1);
			return reportPaginator;
		}

		public abstract ReportData GetData();

		private string GetXaml()
		{
			StreamResourceInfo info = Application.GetResourceStream(ResourceHelper.ComposeResourceUri(GetType().Assembly, string.Format(TemplateFormat, GetType().Name)));
			using (var reader = new StreamReader(info.Stream))
				return reader.ReadToEnd();
		}
	}
}
