using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using ReportsModule.ReportProviders;

namespace ReportsModule.ViewModels
{
	public class ReportsViewModel : ViewPartViewModel
	{
		public ReportsViewModel()
		{
			RefreshCommand = new RelayCommand(OnRefresh, CanRefresh);
			FilterCommand = new RelayCommand(OnFilter, CanFilter);
		}

		public List<ReportType> AvailableReportTypes
		{
			get { return Enum.GetValues(typeof(ReportType)).Cast<ReportType>().ToList(); }
		}

		private DocumentPaginator _documentPaginator;
		public DocumentPaginator DocumentPaginator
		{
			get { return _documentPaginator; }
			private set
			{
				_documentPaginator = value;
				OnPropertyChanged("DocumentPaginator");
				OnPropertyChanged("DocumentWidth");
				OnPropertyChanged("DocumentHeight");
			}
		}
		public double DocumentWidth
		{
			get { return DocumentPaginator == null ? 0 : DocumentPaginator.PageSize.Width; }
		}
		public double DocumentHeight
		{
			get { return DocumentPaginator == null ? 0 : DocumentPaginator.PageSize.Height; }
		}

		private ReportType? _selectedReportName;
		public ReportType? SelectedReportName
		{
			get { return _selectedReportName; }
			set
			{
				_selectedReportName = value;
				OnPropertyChanged("SelectedReportName");
				OnPropertyChanged("IsJournalReport");
				RefreshCommand.Execute();
			}
		}

		public RelayCommand RefreshCommand { get; private set; }
		private void OnRefresh()
		{
			DateTime dt1 = DateTime.Now;
			using (new WaitWrapper())
			{
				DeviceListReport provider = new DeviceListReport();
				DocumentPaginator = provider.GenerateReport();
			}
			DateTime dt2 = DateTime.Now;
			Console.WriteLine("Total: {0}", dt2 - dt1);
			Console.WriteLine("Page Count: {0}", DocumentPaginator.PageCount);
		}
		private bool CanRefresh()
		{
			return true;
		}

		public RelayCommand FilterCommand { get; private set; }
		private void OnFilter()
		{
		}
		private bool CanFilter()
		{
			return SelectedReportName == ReportType.ReportJournal;
		}

	}
}