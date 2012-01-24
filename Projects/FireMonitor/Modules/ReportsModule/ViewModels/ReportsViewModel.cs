using System.Collections.Generic;
using Infrastructure.Common;
using ReportsModule.Reports;
using SAPBusinessObjects.WPF.Viewer;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using Infrastructure;

namespace ReportsModule.ViewModels
{
    public class ReportsViewModel : RegionViewModel
    {
        public ReportsViewModel()
        {
            ReportNames = new List<string>();
            ReportNames.Add("Блоки индикации");
            ReportNames.Add("Журнал событий");
            ReportNames.Add("Количество устройств по типам");
            ReportNames.Add("Параметры устройств");
            ReportNames.Add("Список устройств");
            //ReportWindow = new Window();
            //CrystalReportsViewer = new CrystalReportsViewer();
            //ReportWindow.Content = CrystalReportsViewer;
            //ReportViewerSettings(CrystalReportsViewer);
            //ReportContent = CrystalReportsViewer;
        }

        void ShowCrystalReport(BaseReport baseReport)
        {
            baseReport.LoadData();
            CrystalReportsViewer.ViewerCore.ReportSource = baseReport.CreateCrystalReportDocument();
            ReportWindow.Show();
        }

        void ReportViewerSettings(CrystalReportsViewer crystalReportsViewer)
        {
            crystalReportsViewer.ToggleSidePanel = SAPBusinessObjects.WPF.Viewer.Constants.SidePanelKind.None;
            crystalReportsViewer.ShowLogo = false;
            crystalReportsViewer.ShowRefreshButton = true;
            crystalReportsViewer.ShowSearchTextButton = true;
            crystalReportsViewer.ShowStatusbar = true;
            crystalReportsViewer.ShowToolbar = true;
            crystalReportsViewer.ViewerCore.Zoom(79);
            crystalReportsViewer.ViewerCore.SelectionMode = Constants.ObjectSelectionMode.Multiple;
            crystalReportsViewer.ShowToggleSidePanelButton = false;
            crystalReportsViewer.Loaded +=new RoutedEventHandler(crystalReportsViewer_Loaded);
            crystalReportsViewer.Refresh += new RefreshEventHandler(OnRefresh);
        }

        void crystalReportsViewer_Loaded(object sender, RoutedEventArgs e)
        {
            var crystalReportsViewer = sender as CrystalReportsViewer;
            if (crystalReportsViewer != null)
            {
                var stb = crystalReportsViewer.FindName("statusBar") as StatusBar;
                int i = 0;
                foreach (var item in stb.Items)
                {
                    var btn3 = ((StatusBarItem)item).Content as Button;
                    if ((i == 4)&&(btn3 != null))
                    {
                        btn3.Focus();
                    }
                    i++;
                }
                stb.Items.MoveCurrentToPosition(4);
            }
        }

        public void OnUpdate()
        {
            if (CrystalReportsViewer != null)
            {
                var stb = CrystalReportsViewer.FindName("statusBar") as StatusBar;
                int i = 0;
                foreach (var item in stb.Items)
                {
                    var btn3 = ((StatusBarItem)item).Content as Button;
                    if ((i == 4) && (btn3 != null))
                    {
                        btn3.Focus();
                    }
                    i++;
                }
                stb.Items.MoveCurrentToPosition(4);
            }
        }

        void OnRefresh(object obj, ViewerEventArgs e)
        {
            var crystalReportsViewer = obj as CrystalReportsViewer;
            if (crystalReportsViewer != null)
            {
                var stb = crystalReportsViewer.FindName("statusBar") as StatusBar;
                int i = 0;
                foreach (var item in stb.Items)
                {
                    var btn3 = ((StatusBarItem)item).Content as Button;
                    if ((i == 4) && (btn3 != null))
                    {
                        btn3.Focus();
                    }
                    i++;
                }
                stb.Items.MoveCurrentToPosition(4);
            }
        }

        object _reportContent;
        public object ReportContent
        {
            get { return _reportContent; }
            set
            {
                _reportContent = value;
                OnPropertyChanged("ReportContent");
            }
        }

        public CrystalReportsViewer CrystalReportsViewer { get; private set; }
        public List<string> ReportNames { get; private set; }
        public Window ReportWindow { get; private set; }

        string _selectedReportName;
        public string SelectedReportName
        {
            get { return _selectedReportName; }
            set
            {
                _selectedReportName = value;
                OnPropertyChanged("SelectedReportName");

                switch(value)
                {
                    case "Блоки индикации":
                        ShowCrystalReport(new ReportIndicationBlock());
                        return;

                    case "Журнал событий":
                        ShowCrystalReport(new ReportJournal());
                        return;

                    case "Количество устройств по типам":
                        ShowCrystalReport(new ReportDriverCounter());
                        return;

                    case "Параметры устройств":
                        ShowCrystalReport(new ReportDeviceParams());
                        return;

                    case "Список устройств":
                        ShowCrystalReport(new ReportDevicesList());
                        return;
                }
            }
        }
    }
}