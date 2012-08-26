using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using Infrastructure;
using Infrastructure.Events;
using ReportsModule.Views;

namespace ReportsModule.ViewModels
{
	public class IndicatorViewModel : BaseViewModel
	{
		public IndicatorViewModel()
		{
			ReportsCommand = new RelayCommand(OnReports);
		}
		public RelayCommand ReportsCommand { get; private set; }
		void OnReports()
		{
			var _reportControlView = new ReportControlView();
			_reportControlView.Show();
		}
	}
}
