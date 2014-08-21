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
			_viewer.PageViews[0].PageNumber = 0;
		}

		private void Previous_Click(object sender, RoutedEventArgs e)
		{
			if (_viewer.PageViews[0].PageNumber > 0)
				_viewer.PageViews[0].PageNumber--;
		}

		private void Next_Click(object sender, RoutedEventArgs e)
		{
			if (_viewer.PageViews[0].PageNumber < _viewer.PageViews[0].DocumentPaginator.PageCount - 1)
				_viewer.PageViews[0].PageNumber++;
		}
	}
}
