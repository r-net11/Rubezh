using Infrastructure.Common;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using ReportsModule.Views;
using System.Windows.Documents;
using System.Windows.Markup;
using System.IO;
using System.Collections.Generic;
using FiresecAPI.Models;
using System;
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
				OnPropertyChanged("Document");
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
			DeviceListReport provider = new DeviceListReport();
			DocumentPaginator = provider.GenerateReport();
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