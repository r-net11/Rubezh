using System.Windows.Controls;
using ReportsModule.Reports;
using SAPBusinessObjects.WPF.Viewer;

namespace ReportsModule.Views
{
    public partial class ReportsView : UserControl
    {
        public ReportsView()
        {
            InitializeComponent();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem tabItem = e.Source as TabItem;
            if (tabItem == null) { return; }
            TabControl tabControl = sender as TabControl;

            if (tabControl.SelectedItem == e.Source) { return; }
            tabItem.Content = ShowReport(new ReportIndicationBlock());
        }

        object ShowReport(BaseReport report)
        {
            report.LoadData();
            return report.CreateCrystalReportViewer();
        }

        private void TabControl_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //TabItem tabItem = e.Source as TabItem;
            //if (tabItem == null) { return; }
            //TabControl tabControl = sender as TabControl;

            //if (tabControl.SelectedItem == e.Source) { return; }
            //tabItem.Content = ShowReport(new ReportIndicationBlock());
        }

        private void ContentControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }
}