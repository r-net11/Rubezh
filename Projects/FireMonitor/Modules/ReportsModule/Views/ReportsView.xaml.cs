using System.Windows.Controls;
using System.Windows;
using System;
using Infrastructure.Common;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Threading;
using Infrastructure.Common.Windows;

namespace ReportsModule.Views
{
	public partial class ReportsView : UserControl, INotifyPropertyChanged
	{
		private double _initialScale = 1;
		private BackgroundWorker _worker = null;

		public static readonly DependencyProperty DocumentPaginatorProperty = DependencyProperty.RegisterAttached("DocumentPaginator", typeof(DocumentPaginator), typeof(ReportsView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
		public static void SetDocumentPaginator(UIElement element, double value)
		{
			element.SetValue(DocumentPaginatorProperty, value);
		}
		public static DocumentPaginator GetDocumentPaginator(UIElement element)
		{
			return (DocumentPaginator)element.GetValue(DocumentPaginatorProperty);
		}

		public ReportsView()
		{
			InitializeComponent();
			slider.ValueChanged += OnSliderValueChanged;
			Loaded += new RoutedEventHandler(OnLoaded);
			_scrollViewer.PreviewMouseWheel += new MouseWheelEventHandler(_scrollViewer_PreviewMouseWheel);

			FirstPageCommand = new RelayCommand(OnFirstPage, CanFirstPage);
			NextPageCommand = new RelayCommand(OnNextPage, CanNextPage);
			PreviousPageCommand = new RelayCommand(OnPreviousPage, CanPreviousPage);
			LastPageCommand = new RelayCommand(OnLastPage, CanLastPage);
			FitToWidthCommand = new RelayCommand(OnFitToWidth, CanFitToWidth);
			FitlToHeightCommand = new RelayCommand(OnFitlToHeight, CanFitlToHeight);
			FitToPageCommand = new RelayCommand(OnFitToPage, CanFitToPage);
			PrintReportCommand = new RelayCommand(OnPrintReport, CanPrintReport);
			ZoomInCommand = new RelayCommand(OnZoomIn, CanZoomIn);
			ZoomOutCommand = new RelayCommand(OnZoomOut, CanZoomOut);

			DependencyPropertyDescriptor paginatorDescr = DependencyPropertyDescriptor.FromProperty(ReportsView.DocumentPaginatorProperty, typeof(ReportsView));
			if (paginatorDescr != null)
				paginatorDescr.AddValueChanged(this, DocumentPaginatorChanged);
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			Reset();
		}
		public void Reset()
		{
			Scale = _initialScale;
		}

		public RelayCommand ZoomInCommand { get; private set; }
		private void OnZoomIn()
		{
			slider.Value += 0.1;
		}
		private bool CanZoomIn()
		{
			return slider.Value < slider.Maximum;
		}

		public RelayCommand ZoomOutCommand { get; private set; }
		private void OnZoomOut()
		{
			slider.Value -= 0.1;
		}
		private bool CanZoomOut()
		{
			return slider.Value > slider.Minimum;
		}

		private void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (e.NewValue == 0)
				return;
			Scale = slider.Value * _initialScale;
		}

		public RelayCommand FirstPageCommand { get; private set; }
		private void OnFirstPage()
		{
			_viewer.FirstPage();
			OnPropertyChanged("CurrentPageNumber");
		}
		private bool CanFirstPage()
		{
			return _viewer.CanGoToPreviousPage;
		}

		public RelayCommand PreviousPageCommand { get; private set; }
		private void OnPreviousPage()
		{
			_viewer.PreviousPage();
			OnPropertyChanged("CurrentPageNumber");
		}
		private bool CanPreviousPage()
		{
			return _viewer.CanGoToPreviousPage;
		}

		public RelayCommand NextPageCommand { get; private set; }
		private void OnNextPage()
		{
			_viewer.NextPage();
			OnPropertyChanged("CurrentPageNumber");
		}
		private bool CanNextPage()
		{
			return _viewer.CanGoToNextPage;
		}

		public RelayCommand LastPageCommand { get; private set; }
		private void OnLastPage()
		{
			_viewer.LastPage();
			OnPropertyChanged("CurrentPageNumber");
		}
		private bool CanLastPage()
		{
			return _viewer.CanGoToNextPage;
		}

		public RelayCommand PrintReportCommand { get; private set; }
		private void OnPrintReport()
		{
			_viewer.Print();
		}
		private bool CanPrintReport()
		{
			return _viewer.PageCount > 0;
		}

		public RelayCommand FitToWidthCommand { get; private set; }
		private void OnFitToWidth()
		{
			Scale = (_scrollViewer.ActualWidth - SystemParameters.VerticalScrollBarWidth) / _viewer.Width;
		}
		private bool CanFitToWidth()
		{
			return DocumentPaginator != null;
		}
		public RelayCommand FitlToHeightCommand { get; private set; }
		private void OnFitlToHeight()
		{
			Scale = (_scrollViewer.ActualHeight - SystemParameters.HorizontalScrollBarHeight) / _viewer.Height;
		}
		private bool CanFitlToHeight()
		{
			return DocumentPaginator != null;
		}
		public RelayCommand FitToPageCommand { get; private set; }
		private void OnFitToPage()
		{
			Scale = 1;
		}
		private bool CanFitToPage()
		{
			return DocumentPaginator != null;
		}

		public int TotalPageNumber
		{
			get
			{
				int count = 0;
				ApplicationService.Invoke(() => count = _viewer.PageCount);
				return count;
			}
		}
		public int CurrentPageNumber
		{
			get
			{
				OnPropertyChanged("TotalPageNumber");
				return _viewer.MasterPageNumber;
			}
			set
			{
				_viewer.GoToPage(value);
				OnPropertyChanged("CurrentPageNumber");
			}
		}
		public double PageBorderThickness
		{
			get { return 2.0 / Scale; }
		}
		private DocumentPaginator DocumentPaginator
		{
			get { return GetDocumentPaginator(this); }
		}

		#region INotifyPropertyChanged Members and helper

		private readonly NotifyPropertyChangedHelper _propertyChangeHelper = new NotifyPropertyChangedHelper();

		public event PropertyChangedEventHandler PropertyChanged
		{
			add { _propertyChangeHelper.Add(value); }
			remove { _propertyChangeHelper.Remove(value); }
		}

		protected void SetValue<T>(ref T field, T value, params string[] propertyNames)
		{
			_propertyChangeHelper.SetValue(this, ref field, value, propertyNames);
		}

		public void OnPropertyChanged(string propertyName)
		{
			_propertyChangeHelper.NotifyPropertyChanged(this, propertyName);
		}

		#endregion

		private double Scale
		{
			get { return scaleTransform.ScaleX; }
			set
			{
				if (slider.Value * _initialScale == value)
				{
					scaleTransform.ScaleX = value;
					scaleTransform.ScaleY = value;
					OnPropertyChanged("PageBorderThickness");
				}
				else
					slider.Value = value / _initialScale;
			}
		}

		private void _scrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (_scrollViewer.ContentVerticalOffset == _scrollViewer.ScrollableHeight && e.Delta < 0 && NextPageCommand.CanExecute(null))
			{
				NextPageCommand.Execute();
				_scrollViewer.ScrollToTop();
			}
			else if (_scrollViewer.ContentVerticalOffset == 0 && e.Delta > 0 && PreviousPageCommand.CanExecute(null))
			{
				PreviousPageCommand.Execute();
				_scrollViewer.ScrollToBottom();
			}
		}
		private void DocumentPaginatorChanged(object sender, EventArgs e)
		{
			DateTime dt1 = DateTime.Now;
			if (_worker != null)
			{
				_worker.CancelAsync();
				_worker = null;
			}
			DateTime dt3 = DateTime.Now;
			//foreach (var pageView in _viewer.PageViews)
			//    pageView.DocumentPaginator = null;
			//_viewer.Document = null;
			//_viewer.GoToPage(0);
			DateTime dt4 = DateTime.Now;
			DocumentPaginator documentPaginator = GetDocumentPaginator(this);
			DateTime dt5 = DateTime.Now;
			if (documentPaginator != null)
			{
				//_viewer.Document = documentPaginator.Source;
				foreach (var pageView in _viewer.PageViews)
					pageView.DocumentPaginator = documentPaginator;
			}
			DateTime dt6 = DateTime.Now;
			Scale = _initialScale;
			_scrollViewer.ScrollToTop();
			_scrollViewer.ScrollToLeftEnd();
			_viewer.UpdateLayout();
			DateTime dt7 = DateTime.Now;
			OnPropertyChanged("CurrentPageNumber");
			if (_viewer.Document != null)
			{
				_worker = new BackgroundWorker();
				_worker.WorkerSupportsCancellation = true;
				_worker.DoWork += new DoWorkEventHandler(UpdatePageCountWork);
				_worker.RunWorkerAsync();
			}
			DateTime dt2 = DateTime.Now;
			Console.WriteLine("Refresh view: {0}", dt2 - dt1);
			Console.WriteLine("-- Cleanup Document: {0}", dt4 - dt3);
			Console.WriteLine("-- Get DocumentPaginator: {0}", dt5 - dt4);
			Console.WriteLine("-- Set Document: {0}", dt6 - dt5);
			Console.WriteLine("-- Update Layout: {0}", dt7 - dt6);
		}

		private void UpdatePageCountWork(object sender, DoWorkEventArgs e)
		{
			var worker = (BackgroundWorker)sender;
			int count = 0;
			while (count != TotalPageNumber && !worker.CancellationPending)
			{
				count = TotalPageNumber;
				ApplicationService.Invoke(() => OnPropertyChanged("TotalPageNumber"));
				Thread.Sleep(100);
				if (count == TotalPageNumber && !worker.CancellationPending)
					Thread.Sleep(200);
			}
		}
	}
}