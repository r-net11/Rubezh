using System.Windows.Controls;
using System.Windows;
using System;
using Infrastructure.Common;
using System.ComponentModel;
using System.Windows.Data;

namespace ReportsModule.Views
{
	public partial class ReportsView : UserControl, INotifyPropertyChanged
	{
		private double initialScale = 1;

		public ReportsView()
		{
			InitializeComponent();
			slider.ValueChanged += OnSliderValueChanged;
			Loaded += new RoutedEventHandler(OnLoaded);

			FirstPageCommand = new RelayCommand(OnFirstPage, CanFirstPage);
			NextPageCommand = new RelayCommand(OnNextPage, CanNextPage);
			PreviousPageCommand = new RelayCommand(OnPreviousPage, CanPreviousPage);
			LastPageCommand = new RelayCommand(OnLastPage, CanLastPage);
			WidthToPageCommand = new RelayCommand(OnWidthToPage, CanWidthToPage);
			PrintReportCommand = new RelayCommand(OnPrintReport, CanPrintReport);
			ZoomInCommand = new RelayCommand(OnZoomIn, CanZoomIn);
			ZoomOutCommand = new RelayCommand(OnZoomOut, CanZoomOut);
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			Reset();
		}
		public void Reset()
		{
			slider.Value = 1;
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

			scaleTransform.ScaleX = slider.Value * initialScale;
			scaleTransform.ScaleY = slider.Value * initialScale;
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

		public RelayCommand WidthToPageCommand { get; private set; }
		private void OnWidthToPage()
		{
			double scaleX = (_scrollViewer.ActualWidth - 30) / _viewer.Width;
			scaleTransform.ScaleX = scaleX;
			scaleTransform.ScaleY = scaleX;
		}
		private bool CanWidthToPage()
		{
			return true;
		}

		public int TotalPageNumber
		{
			get { return _viewer.PageCount; }
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
	}
}