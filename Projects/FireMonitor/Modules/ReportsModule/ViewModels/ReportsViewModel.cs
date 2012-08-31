using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Printing;
using System.Windows.Documents;
using System.Windows.Xps;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
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
			PrintReportCommand = new RelayCommand(OnPrintReport, CanPrintReport);
			Reports = new List<BaseReport>()
			{
				new DeviceParamsReport(),
				new DeviceListReport(),
				new DriverCounterReport(),
				new IndicationBlockReport(),
				new JournalReport(),
			};
			SelectedReport = null;
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

		public List<BaseReport> Reports { get; private set; }
		private BaseReport _selectedReport;
		public BaseReport SelectedReport
		{
			get { return _selectedReport; }
			set
			{
				_selectedReport = value;
				OnPropertyChanged("SelectedReport");
				RefreshCommand.Execute();
			}
		}
		private bool _inProgress;
		public bool InProgress
		{
			get { return _inProgress; }
			set
			{
				_inProgress = value;
				OnPropertyChanged("InProgress");
			}
		}

		public RelayCommand RefreshCommand { get; private set; }
		private void OnRefresh()
		{
			if (SelectedReport != null)
				ApplicationService.BeginInvoke((Action)(() =>
					{
						DateTime dt = DateTime.Now;
						try
						{
							LoadingService.ShowProgress("Подождите...", "Идет построение отчета", 0);
							InProgress = true;
							DocumentPaginator = SelectedReport.GenerateReport();
						}
						finally
						{
							LoadingService.Close();
							InProgress = false;
						}
						Debug.WriteLine("Total: {0}", DateTime.Now - dt);
						Debug.WriteLine("Page Count: {0}", DocumentPaginator == null ? 0 : DocumentPaginator.PageCount);
					}));
			else
				DocumentPaginator = null;
		}
		private bool CanRefresh()
		{
			return SelectedReport != null;
		}

		public RelayCommand FilterCommand { get; private set; }
		private void OnFilter()
		{
			SelectedReport.Filter(RefreshCommand);
		}
		private bool CanFilter()
		{
			return SelectedReport != null && SelectedReport.IsFilterable;
		}

		public RelayCommand PrintReportCommand { get; private set; }
		private void OnPrintReport()
		{
			PrintDocumentImageableArea documentImageableArea = null;
			XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(ref documentImageableArea);
			if (writer != null && documentImageableArea != null)
				writer.WriteAsync(DocumentPaginator);
		}
		private bool CanPrintReport()
		{
			return DocumentPaginator != null && DocumentPaginator.PageCount > 0;
		}
	}
}