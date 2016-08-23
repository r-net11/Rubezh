using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraReports.UserDesigner.Native;
using System.Drawing.Design;
using System.Windows.Forms;
using CommandVisibility = DevExpress.XtraReports.UserDesigner.CommandVisibility;

namespace ReportSystem.UI
{
	public partial class PassCardDesignerControl : UserControl
	{
		public XtraReport CurrentReport { get; set; }

		public PassCardDesignerControl()
		{
			InitializeComponent();
			reportDesigner1.DesignPanelLoaded += Loaded;
		}

		private void Loaded(object sender, DesignerLoadedEventArgs e)
		{
			IToolboxService ts = (IToolboxService)e.DesignerHost.GetService(typeof(IToolboxService));

			// Get a collection of toolbox items.
			ToolboxItemCollection coll = ts.GetToolboxItems();

			// Iterate through toolbox items.
			foreach (ToolboxItem item in coll)
			{
				// Add the "Cool" prefix to all toolbox item names.
				var itemName = (item as LocalizableToolboxItem).TypeName;
				if (//itemName == typeof(XRGauge).FullName
					//||
					itemName == typeof(XRChart).FullName
					|| itemName == typeof(XRBarCode).FullName
					|| itemName == typeof(XRZipCode).FullName
					|| itemName == typeof(XRSparkline).FullName
					|| itemName == typeof(XRPivotGrid).FullName
					|| itemName == typeof(XRSubreport).FullName
					|| itemName == typeof(XRTableOfContents).FullName
					|| itemName == typeof(XRPageInfo).FullName
					|| itemName == typeof(XRPageBreak).FullName
					|| itemName == typeof(XRCrossBandLine).FullName
					|| itemName == typeof(XRCrossBandBox).FullName
					|| itemName == typeof(XRCheckBox).FullName
					|| itemName == typeof(XRRichText).FullName
					|| itemName == typeof(XRTable).FullName
					|| itemName == typeof(XRPanel).FullName)
					ts.RemoveToolboxItem(item);
			}
		}

		public void OpenCurrentReport()
		{
			reportDesigner1.OpenReport(CurrentReport);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.ShowScriptsTab, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.ShowHTMLViewTab, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.ShowDesignerTab, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.ShowPreviewTab, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.AddNewDataSource, CommandVisibility.None);

			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.AddNewDataSource, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.AddNewDataSource, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.InsertBottomMarginBand, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.InsertDetailBand, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.InsertDetailReport, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.InsertGroupFooterBand, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.InsertGroupHeaderBand, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.InsertPageFooterBand, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.InsertPageHeaderBand, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.InsertReportFooterBand, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.InsertReportHeaderBand, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.InsertTopMarginBand, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.MdiCascade, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.MdiTileHorizontal, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.MdiTileVertical, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.NewReport, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.NewReportWizard, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.OpenFile, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.OpenRemoteReport, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.OpenSubreport, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.SaveAll, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.SaveFile, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.SaveFileAs, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.ShowTabbedInterface, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.ShowWindowInterface, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.UploadNewRemoteReport, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.VerbEditBands, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.VerbEditBindings, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.VerbEditText, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.VerbExecute, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.VerbExport, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.VerbImport, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.VerbLoadReportTemplate, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.VerbPivotGridDesigner, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.VerbReportWizard, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.VerbRtfClear, CommandVisibility.None);
			reportDesigner1.ActiveDesignPanel.SetCommandVisibility(ReportCommand.VerbRtfLoadFile, CommandVisibility.None);


			reportDesigner1.SetCommandVisibility(ReportCommand.ShowScriptsTab, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.ShowHTMLViewTab, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.ShowDesignerTab, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.AddNewDataSource, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.AddNewDataSource, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.InsertBottomMarginBand, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.InsertDetailBand, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.InsertDetailReport, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.InsertGroupFooterBand, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.InsertGroupHeaderBand, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.InsertPageFooterBand, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.InsertPageHeaderBand, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.InsertReportFooterBand, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.InsertReportHeaderBand, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.InsertTopMarginBand, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.MdiCascade, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.MdiTileHorizontal, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.MdiTileVertical, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.NewReport, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.NewReportWizard, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.OpenFile, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.OpenRemoteReport, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.OpenSubreport, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.SaveAll, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.SaveFile, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.SaveFileAs, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.ShowTabbedInterface, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.ShowWindowInterface, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.UploadNewRemoteReport, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.VerbEditBands, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.VerbEditBindings, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.VerbEditText, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.VerbExecute, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.VerbExport, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.VerbImport, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.VerbLoadReportTemplate, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.VerbPivotGridDesigner, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.VerbReportWizard, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.VerbRtfClear, CommandVisibility.None);
			reportDesigner1.SetCommandVisibility(ReportCommand.VerbRtfLoadFile, CommandVisibility.None);


		}
	}
}
