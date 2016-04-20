using DevExpress.XtraReports.UI;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace Resurs.ViewModels
{
	public class ReportDesignerViewModel : SaveCancelDialogViewModel
	{
		public static XtraReport Report { get; set; }
		public ReportDesignerViewModel(XtraReport report)
		{
			Title = "Дизайнер отчетов";
			Report = report;
		}
		protected override bool Save()
		{
			return base.Save();
		}
	}
}