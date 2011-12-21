using System.Collections.Generic;
using Common;
using CrystalDecisions.CrystalReports.Engine;
using FiresecClient;
using SAPBusinessObjects.WPF.Viewer;

namespace ReportsModule.Reports
{
    public class BaseReportGeneric<T> : BaseReport
    {
        protected string ReportFileName;
        protected ReportDocument reportDocument;

        public BaseReportGeneric()
        {
            DataList = new List<T>();
            reportDocument = new ReportDocument();
        }

        protected List<T> _dataList;
        public List<T> DataList { get; protected set; }

        public override CrystalReportsViewer CreateCrystalReportViewer()
        {
            if (DataList.IsNotNullOrEmpty() == false)
                return new CrystalReportsViewer();

            reportDocument = new ReportDocument();
            reportDocument.Load(FileHelper.GetReportFilePath(ReportFileName));
            reportDocument.SetDataSource(DataList);

            var crystalReportsViewer = new CrystalReportsViewer();
            crystalReportsViewer.ViewerCore.ReportSource = reportDocument;
            crystalReportsViewer.ShowLogo = false;
            crystalReportsViewer.ShowToggleSidePanelButton = true;
            crystalReportsViewer.ToggleSidePanel = SAPBusinessObjects.WPF.Viewer.Constants.SidePanelKind.None;
            
            return crystalReportsViewer;
        }

        //public void SetReportViewMode(CrystalReportsViewer crystalReportsViewer)
        //{
        //    foreach (Control control in crystalReportsViewer.)
        //    {
        //        if (control is PageView)
        //        {
        //            foreach (Control controlInPage in control.Controls)
        //            {
        //                if (controlInPage is TabControl)
        //                    foreach (TabPage tabPage in (controlInPage as TabControl).TabPages)
        //                    {
        //                        if (tabPage.Text == "Main Report")
        //                        {
        //                            tabPage.Text = newName;
        //                        }
        //                    }
        //            }
        //        }
        //        if (control is ToolStrip)
        //        {
        //            int i = 0;
        //            foreach (ToolStripItem item in (control as ToolStrip).Items)
        //            {
        //                if (item.ToolTipText == "Zoom")
        //                {
        //                    int y = 0;
        //                    ToolStripDropDownButton button = item as ToolStripDropDownButton;
        //                    foreach (ToolStripMenuItem mi in button.DropDownItems)
        //                    {
        //                        mi.Text = this.zoomlist[y].ToString();
        //                        ++y;
        //                    }
        //                }
        //                item.ToolTipText = this.tooltip.ToString();
        //                ++i;
        //            }
        //        }
        //    }
        //}
    }
}
