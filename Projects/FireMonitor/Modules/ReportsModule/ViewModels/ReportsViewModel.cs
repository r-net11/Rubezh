using System.Collections.Generic;
using Infrastructure.Common;
using ReportsModule.Reports;
using SAPBusinessObjects.WPF.Viewer;
using System.Windows.Controls.Primitives;

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
        }

        void ShowCrystalReport(BaseReport baseReport)
        {
            baseReport.LoadData();
            ReportContent = baseReport.CreateCrystalReportViewer();
            ReportViewerSettings((CrystalReportsViewer)ReportContent);
        }

        void ReportViewerSettings(CrystalReportsViewer crystalReportsViewer)
        {
            crystalReportsViewer.ToggleSidePanel = SAPBusinessObjects.WPF.Viewer.Constants.SidePanelKind.GroupTree;
            var stb = crystalReportsViewer.FindName("mainToolbar") as System.Windows.Controls.ToolBar;
            
            crystalReportsViewer.ShowLogo = false;
            crystalReportsViewer.ShowToggleSidePanelButton = true;
            crystalReportsViewer.ShowRefreshButton = true;
            crystalReportsViewer.ShowSearchTextButton = true;
            crystalReportsViewer.ShowStatusbar = true;
            crystalReportsViewer.ShowToolbar = true;
            crystalReportsViewer.ViewerCore.Zoom(79);
            crystalReportsViewer.ViewerCore.SelectionMode = Constants.ObjectSelectionMode.Multiple;
            crystalReportsViewer.Refresh += new RefreshEventHandler(OnRefresh);
            if (stb != null)
            {
                stb.IsEnabled = true;
            }
        }

        void OnRefresh(object obj,ViewerEventArgs e)
        {
            var view = obj as CrystalReportsViewer;
            if (view != null)
            {
                view.ViewerCore.CurrentPageNumber = 3;
            }
            var stb = view.FindName("mainToolbar") as System.Windows.Controls.ToolBar;
            if (stb != null)
            {
                stb.IsEnabled = true;
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

        public List<string> ReportNames { get; private set; }

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