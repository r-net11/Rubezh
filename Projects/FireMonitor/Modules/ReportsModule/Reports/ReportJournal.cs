using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Xps.Packaging;
using CodeReason.Reports;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using JournalModule.ViewModels;

namespace ReportsModule.Reports
{
    public class ReportJournal
    {
        public ReportJournal()
        {
            _journalList = new List<Journal>();
        }

        List<Journal> _journalList;
        DateTime _startDate;
        DateTime _endDate;

        public XpsDocument CreateReport()
        {
            Initialize();

            var reportDocument = new ReportDocument();
            //string path = @"H:\Rubezh\Projects\FireMonitor\Modules\ReportsModule\ReportTemplates\JournalFlowDocument.xaml";
            string path = @"ReportTemplates/JournalFlowDocument.xaml";
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                reportDocument.XamlData = new StreamReader(fileStream).ReadToEnd();
            }

            var DateNow = DateTime.Now;
            var data = new ReportData();
            data.ReportDocumentValues.Add("PrintDateStart", _startDate);
            data.ReportDocumentValues.Add("PrintDateEnd", _endDate);
            data.ReportDocumentValues.Add("PrintDateNow", DateNow);

            var dataTable = new DataTable("JournalList");
            dataTable.Columns.Add();
            dataTable.Columns.Add();
            dataTable.Columns.Add();
            dataTable.Columns.Add();
            dataTable.Columns.Add();
            dataTable.Columns.Add();
            dataTable.Columns.Add();
            foreach (var journal in _journalList)
            {
                dataTable.Rows.Add(journal.DeviceTime, journal.SystemTime, journal.ZoneName, journal.Description, journal.Device, journal.Panel, journal.User);
            }
            data.DataTables.Add(dataTable);
            return reportDocument.CreateXpsDocument(data);
        }

        void Initialize()
        {
            ReportArchiveFilter _reportArchiveFilter = new ReportArchiveFilter();
            _reportArchiveFilter.ShowFilter();
            foreach (var journalRecord in _reportArchiveFilter.JournalRecords)
            {
                _journalList.Add(new Journal()
                {
                    DeviceTime = journalRecord.DeviceTime,
                    SystemTime = journalRecord.SystemTime,
                    ZoneName = journalRecord.ZoneName,
                    Description = journalRecord.Description,
                    Device = journalRecord.Device,
                    Panel = journalRecord.Panel,
                    User = journalRecord.User
                });
            }

            _startDate = _reportArchiveFilter.StartDate;
            _endDate = _reportArchiveFilter.EndDate;
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

    internal class ReportArchiveFilter
    {
        ArchiveFilter _archiveFilter;

        public ReportArchiveFilter()
        {
            _archiveFilter = new ArchiveFilter()
            {
                EndDate = DateTime.Now,
                StartDate = DateTime.Now.AddDays(-1),
                UseSystemDate = false
            };
            IsFilterOn = false;
        }

        public DateTime StartDate
        {
            get { return _archiveFilter.StartDate; }
        }

        public DateTime EndDate
        {
            get { return _archiveFilter.EndDate; }
        }

        bool _isFilterOn;
        public bool IsFilterOn
        {
            get { return _isFilterOn; }
            set
            {
                if (value)
                {
                    ApplyFilter();
                }

                _isFilterOn = value;
            }
        }

        List<JournalRecordViewModel> _journalRecords;
        public List<JournalRecordViewModel> JournalRecords
        {
            get { return _journalRecords; }
            private set
            {
                _journalRecords = value;
            }
        }

        void ApplyFilter()
        {
            JournalRecords = new List<JournalRecordViewModel>(
                FiresecManager.GetFilteredArchive(_archiveFilter).
                Select(journalRecord => new JournalRecordViewModel(journalRecord))
            );
        }

        public void ShowFilter()
        {
            var archiveFilterViewModel = new ArchiveFilterViewModel(_archiveFilter);
            if (ServiceFactory.UserDialogs.ShowModalWindow(archiveFilterViewModel))
            {
                _archiveFilter = archiveFilterViewModel.GetModel();
                IsFilterOn = true;
            }
        }
    }
}