using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Xps.Packaging;
using CodeReason.Reports;
using Common;
using FiresecClient;
using Infrastructure.Common;
using System.Windows.Documents;
using System;
using System.Text;
using FiresecAPI.Models;
using JournalModule.ViewModels;
using JournalModule.Views;
using Infrastructure;
using System.Collections.ObjectModel;
using System.Reflection;
using ReportsModule.ViewModels;

namespace ReportsModule.Reports
{
    public class ReportJournal
    {
        List<Journal> JournalList;

        public XpsDocument CreateReport()
        {
            Initialize();

            var reportDocument = new ReportDocument();
            string path = @"H:\Rubezh\Projects\FireMonitor\Modules\ReportsModule\ReportTemplates\JournalFlowDocument.xaml";
            
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                reportDocument.XamlData = new StreamReader(fileStream).ReadToEnd();
            }

            var data = new ReportData();
            var dateTime = DateTime.Now;
            data.ReportDocumentValues.Add("PrintDate", dateTime);

            var dataTable = new DataTable("JournalList");

            Helper.AddDataColumns<Journal>(dataTable);


            
            data.DataTables.Add(dataTable);
            return reportDocument.CreateXpsDocument(data);
        }

        void Initialize()
        {
            var JournalRecords = new ObservableCollection<JournalRecordViewModel>(
                FiresecManager.GetFilteredJournal(new JournalFilter() { LastRecordsCount = 100 }).
                Select(journalRecord => new JournalRecordViewModel(journalRecord))
            );

            var filterViewModel = new ArchiveFilterViewModel();
            ServiceFactory.UserDialogs.ShowModalWindow(filterViewModel);

        }
    }

    internal class Journal
    {
        public string DeviceTime { get; set; }
        public string SystemTime { get; set; }
        public string ZoneName { get; set; }
        public string Description { get; set; }
        public string Device { get; set; }
        public string Panel { get; set; }
        public string User { get; set; }
    }
}