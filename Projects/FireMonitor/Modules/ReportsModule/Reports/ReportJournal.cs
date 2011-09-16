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
            _archiveFilter = new ArchiveFilterViewModel();
            _journalList = new List<Journal>();
            JournalRecords = new List<JournalRecordViewModel>();

            SetDefaultArchiveContent();
        }

        List<Journal> _journalList;
        ArchiveFilterViewModel _archiveFilter;
        public List<JournalRecordViewModel> JournalRecords { get; set; }

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
            data.ReportDocumentValues.Add("PrintDateStart", _archiveFilter.StartDate);
            data.ReportDocumentValues.Add("PrintDateEnd", _archiveFilter.EndDate);
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
            //Helper.AddDataColumns<Journal>(dataTable);
            data.DataTables.Add(dataTable);
            return reportDocument.CreateXpsDocument(data);
        }

        void Initialize()
        {
            //ArchiveFilterViewModel tmpArchiveFilter = new ArchiveFilterViewModel();
            //_archiveFilter.CopyTo(tmpArchiveFilter);
            if (ServiceFactory.UserDialogs.ShowModalWindow(_archiveFilter))
            {
                ApplyFilter();
            }

            foreach (var journalRecord in JournalRecords)
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
        }

        void ApplyFilter()
        {
            ArchiveFilter filter = new ArchiveFilter()
            {
                Descriptions = new List<string>(
                    _archiveFilter.JournalEvents.Where(x => x.IsEnable).Select(x => x.Name)
                ),
                Subsystems = new List<SubsystemType>(
                    _archiveFilter.Subsystems.Where(x => x.IsEnable).Select(x => x.Subsystem)
                ),
                UseSystemDate = _archiveFilter.UseSystemDate,
                StartDate = _archiveFilter.StartDate,
                EndDate = _archiveFilter.EndDate,
            };
            if (filter.Subsystems.Count == 0)
            {
                foreach (SubsystemType subsystem in Enum.GetValues(typeof(SubsystemType)))
                {
                    filter.Subsystems.Add(subsystem);
                }
            }

            JournalRecords = new List<JournalRecordViewModel>(
                FiresecManager.GetFilteredArchive(filter).Select(journalRecord => new JournalRecordViewModel(journalRecord))
            );
        }

        void SetDefaultArchiveContent()
        {
            try
            {
                JournalRecords = new List<JournalRecordViewModel>(
                    FiresecManager.GetFilteredJournal(new JournalFilter() { LastRecordsCount = 100 }).
                    Select(journalRecord => new JournalRecordViewModel(journalRecord))
                );
            }
            catch { ;}
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