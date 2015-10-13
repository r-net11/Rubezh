using DevExpress.Xpf.Printing;
using DevExpress.XtraReports.UI;

namespace Resurs.Reports
{
	public class ReportPreviewModel : XtraReportPreviewModel
	{
		public string SelectedPage
		{
			get { return CurrentPageNumber.ToString(); }
			set
			{
				int intValue;
				if (int.TryParse(value, out intValue) && intValue >= 1 && intValue <= this.PageCount)
					CurrentPageNumber = intValue;
				RaisePropertyChanged(() => SelectedPage);
			}
		}
		public ReportPreviewModel(XtraReport xtraReport)
			: base(xtraReport)
		{
		}
		protected override void OnCurrentPageIndexChanged()
		{
			base.OnCurrentPageIndexChanged();
			RaisePropertyChanged(() => SelectedPage);
		}
	}
}