using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Printing;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Resources;
using CodeReason.Reports;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace ReportsModule.ReportProviders
{
	public abstract class BaseReport : BaseViewModel
	{
		private const string TemplateFormat = "/ReportTemplates/{0}Template.xaml";

		public BaseReport(ReportType reportType)
		{
			ReportType = reportType;
			IsActive = false;
		}

		public ReportType ReportType { get; private set; }
		public string Title
		{
			get { return ReportType.ToDescription(); }
		}
		public abstract bool IsFilterable { get; }
		public virtual bool IsMultiReport
		{
			get { return false; }
		}
		public virtual bool IsEnabled
		{
			get { return true; }
		}
		public bool IsActive { get; set; }

		public DocumentPaginator GenerateReport()
		{
			DateTime dt = DateTime.Now;
			ReportDocument reportDocument = new ReportDocument();
			reportDocument.XamlData = GetXaml();
			DocumentPaginator documentPaginator = IsMultiReport ? (DocumentPaginator)new MultipleReportPaginator(reportDocument, GetMultiData()) : (DocumentPaginator)new ReportPaginator(reportDocument, GetData());
			Debug.WriteLine("Build report: {0}", DateTime.Now - dt);
			return documentPaginator;
		}

		public virtual ReportData GetData()
		{
			return new ReportData();
		}
		public virtual IEnumerable<ReportData> GetMultiData()
		{
			return new ReportData[0];
		}
		public virtual void Filter(RelayCommand refreshCommand)
		{
		}
		public virtual void PreparePrinting(PrintTicket printTicket, Size pageSize)
		{
			printTicket.PageOrientation = pageSize.Height >= pageSize.Width ? PageOrientation.Portrait : PageOrientation.Landscape;
		}

		private string GetXaml()
		{
			StreamResourceInfo info = Application.GetResourceStream(ResourceHelper.ComposeResourceUri(GetType().Assembly, string.Format(TemplateFormat, GetType().Name)));
			using (var reader = new StreamReader(info.Stream))
				return reader.ReadToEnd();
		}
	}
}
