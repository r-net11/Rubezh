using DevExpress.XtraReports.UI;
using Infrastructure.Common.Windows.ViewModels;
using Resurs.Reports;
using System.Collections.Generic;
using System.IO;

namespace Resurs.ViewModels
{
	public class RepxSelectionViewModel : SaveCancelDialogViewModel
	{
		public RepxSelectionViewModel(ReportType reportType)
		{
			var model = ReportHelper.GetDefaultReport(reportType);
			Repxs = new List<RepxViewModel>();
			Repxs.Add(new RepxViewModel(model, "По умолчанию"));
			var path = @"C:\";
			foreach (var filePath in Directory.EnumerateFiles(path, "*.repx"))
			{
				var report = (XtraReport.FromFile(filePath, true));
				if (report is IReport)
				{
					if ((report as IReport).ReportType == reportType)
					Repxs.Add(new RepxViewModel(report,filePath));
				}
			}
		}
		public List<RepxViewModel> Repxs { get; private set; }
		private RepxViewModel _selectedRepx;
		public RepxViewModel SelectedRepx
		{
			get { return _selectedRepx; }
			set
			{
				_selectedRepx = value;
				OnPropertyChanged(() => SelectedRepx);
			}
		}
	}
}