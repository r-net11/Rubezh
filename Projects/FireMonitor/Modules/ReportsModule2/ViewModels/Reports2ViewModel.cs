using System;
using System.Windows.Controls;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using ReportsModule2.DocumentPaginatorModel;
using ReportsModule2.Reports;
using System.Windows.Markup;
using System.Windows.Documents;
using Controls;
using System.Windows;
using MS.Internal.Documents;
using FiresecAPI.Models;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Windows.Xps.Packaging;
using System.Windows.Xps.Serialization;
using System.IO;
using System.Windows.Forms.Integration;
using Microsoft.Reporting.WinForms;
using System.Windows.Forms;
using ReportsModule2.Views;
using Infrastructure.Common.Windows;
using JournalModule.ViewModels;

namespace ReportsModule2.ViewModels
{
	public class Reports2ViewModel : ViewPartViewModel
	{
		public Reports2ViewModel()
		{
			ShowReportCommand = new RelayCommand(OnShowReport);
		}

		public RelayCommand ShowReportCommand { get; private set; }
		void OnShowReport()
		{
			var _reportControlView = new ReportControlView();
			_reportControlView.Show();
		}
	}
}