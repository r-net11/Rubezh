using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using Infrastructure.Common;

namespace ReportsModule.Views
{
	public partial class ReportsView : UserControl, INotifyPropertyChanged
	{
		private double _initialScale = 1;

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
			CurrentPageNumber = 1;
		}
		private bool CanFirstPage()
		{
			return CurrentPageNumber > 1;
		}

		public RelayCommand PreviousPageCommand { get; private set; }
		private void OnPreviousPage()
		{
			CurrentPageNumber--;
		}
		private bool CanPreviousPage()
		{
			return CurrentPageNumber > 1;
		}

		public RelayCommand NextPageCommand { get; private set; }
		private void OnNextPage()
		{
			CurrentPageNumber++;
		}
		private bool CanNextPage()
		{
			return CurrentPageNumber < TotalPageNumber;
		}

		public RelayCommand LastPageCommand { get; private set; }
		private void OnLastPage()
		{
			CurrentPageNumber = TotalPageNumber;
		}
		private bool CanLastPage()
		{
			return CurrentPageNumber < TotalPageNumber;
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
			get { return DocumentPaginator == null ? 0 : DocumentPaginator.PageCount; }
		}
		public int CurrentPageNumber
		{
			get { return DocumentPaginator == null ? 0 : PageView.PageNumber + 1; }
			set
			{
				if (value < 0)
					throw new ApplicationException();
				else if (value > DocumentPaginator.PageCount)
				{
					if (PageView.PageNumber == DocumentPaginator.PageCount - 1)
						throw new ApplicationException();
					else
						value = DocumentPaginator.PageCount;
				}
				using (new WaitWrapper())
					if (PageView.PageNumber != value - 1)
					{
						PageView.PageNumber = value - 1;
						CommandManager.InvalidateRequerySuggested();
						ResetScroll();
						PageView.UpdateLayout();
					}
				OnPropertyChanged("CurrentPageNumber");
				OnPropertyChanged("TotalPageNumber");
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
		private DocumentPageView PageView
		{
			get { return _viewer.PageViews[0]; }
		}
		private void ResetScroll()
		{
			_scrollViewer.ScrollToTop();
			_scrollViewer.ScrollToLeftEnd();
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
			DateTime dt = DateTime.Now;
			PageView.DocumentPaginator = DocumentPaginator;
			PageView.PageNumber = 0;
			Scale = _initialScale;
			ResetScroll();
			PageView.UpdateLayout();
			OnPropertyChanged("CurrentPageNumber");
			OnPropertyChanged("TotalPageNumber");
			Debug.WriteLine("Refresh view: {0}", DateTime.Now - dt);
		}
	}
}