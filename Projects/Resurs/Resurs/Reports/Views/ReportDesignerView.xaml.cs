using DevExpress.XtraReports.UI;
using Resurs.Reports.Templates;
using Resurs.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Resurs.Views
{
	/// <summary>
	/// Interaction logic for ReportDesignerView.xaml
	/// </summary>
	public partial class ReportDesignerView : UserControl
	{
		public ReportDesignerView()
		{
			InitializeComponent();
		}
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			designer.OpenDocument(ReportDesignerViewModel.Report);
		}
	}
}