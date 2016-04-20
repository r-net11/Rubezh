using Infrastructure.Common.Windows.Windows.ViewModels;
using Resurs.Reports;

namespace Resurs.ViewModels
{
	public class ReportFilterViewModel : SaveCancelDialogViewModel
	{
		public ReportFilter Filter { get; set; }
		public ReportFilterViewModel()
		{
			Filter = new ReportFilter();
		}
	}
}