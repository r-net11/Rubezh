using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReportsModule.ViewModels;
using System.Windows.Controls.Primitives;

namespace ReportsModule.Views
{
	/// <summary>
	/// Interaction logic for ReportPreviewView.xaml
	/// </summary>
	public partial class ReportPreviewView : UserControl
	{
		public ReportPreviewView()
		{
			InitializeComponent();
			Loaded += new RoutedEventHandler(ReportPreviewView_Loaded);
		}

		private void ReportPreviewView_Loaded(object sender, RoutedEventArgs e)
		{
			_viewer.PageViews[0].DocumentPaginator = ((ReportPreviewViewModel)DataContext).DocumentPaginator;
		}
		private void ResetScroll()
		{
			_scrollViewer.ScrollToTop();
			_scrollViewer.ScrollToLeftEnd();
		}
		private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (_scrollViewer.ContentVerticalOffset == _scrollViewer.ScrollableHeight && e.Delta < 0 && _nextButton.Command.CanExecute(null))
			{
				_nextButton.Command.Execute(null);
				_scrollViewer.ScrollToTop();
			}
			else if (_scrollViewer.ContentVerticalOffset == 0 && e.Delta > 0 && _previousButton.Command.CanExecute(null))
			{
				_previousButton.Command.Execute(null);
				_scrollViewer.ScrollToBottom();
			}
		}

		private void OnPageConnected(object sender, EventArgs e)
		{
			ResetScroll();
		}
	}
}
