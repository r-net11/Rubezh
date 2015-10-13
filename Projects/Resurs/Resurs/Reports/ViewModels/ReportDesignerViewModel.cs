using DevExpress.Xpf.Reports.UserDesigner;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.UI;
using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;

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
			Infrastructure.Common.Windows.DialogService.ShowModalWindow(new ReceiptViewModel(new Consumer()));
			return base.Save();
		}
	}
}