using System;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace ReportsModule.Views
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public void Inicialize(ReportViewer reportViewer)
        {
            this.reportViewer1 = reportViewer;
            reportViewer1.RefreshReport();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            reportViewer1.Height = Height;
            reportViewer1.Width = Width;
        }

        //private void Form1_Load(object sender, EventArgs e)
        //{
        //    //var dataTabel = reportJournalDataTable.JournalList;
        //    ////var startDate = new ReportParameter("StartDate",reportJournalDataTable.StartDate.ToString(),true);
        //    ////var endDate = new ReportParameter("EndDate", reportJournalDataTable.EndDate.ToString(),true);
        //    //this.reportViewer1.ProcessingMode = ProcessingMode.Local;
        //    //this.reportViewer1.LocalReport.ReportPath = "ReportJournalRDLC.rdlc";
        //    ////this.reportViewer1.LocalReport.SetParameters(new ReportParameter[] { startDate2 });
        //    //this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetJournal", dataTabel));
        //    //this.reportViewer1.RefreshReport();
        //}
    }
}
