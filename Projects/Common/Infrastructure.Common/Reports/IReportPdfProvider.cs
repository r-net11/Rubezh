using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text;
using CodeReason.Reports;

namespace Infrastructure.Common.Reports
{
	public interface IReportPdfProvider
	{
		Rectangle PageFormat { get; }
		ReportData ReportData { get; set; }
		bool CanPrint { get; }
		void Print(Document document);
	}
}
