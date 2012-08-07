using System.Text;
using System.Windows.Xps.Packaging;
using System.IO;
using Microsoft.Reporting.WinForms;
namespace ReportsModule2.Reports
{
	public class BaseReport
	{
		public string ReportFileName { get; protected set; }

		public virtual void LoadData()
		{
		}

		public virtual void CreateFlowDocumentStringBuilder(){ }

		public virtual ReportDataSource CreateDataSource()
		{
			return new ReportDataSource();
		}

		public StringBuilder FlowDocumentStringBuilder { get; set; }
		public string XpsDocumentName { get; set; }
		public XpsDocument XpsDocument
		{
			get
			{
				var xpsDocument = new XpsDocument(XpsDocumentName, FileAccess.Read);
				return xpsDocument;
			}
		}
	}
}