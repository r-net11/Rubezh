using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using CodeReason.Reports;
using System.Data;
using FiresecAPI.Models;
using System.Windows;
using JournalModule.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure.Common;
using FiresecClient;
using FiresecAPI;

namespace ReportsModule.ReportProviders
{
	internal class JournalReport : BaseReport
	{
		private ReportArchiveFilter ReportArchiveFilter { get; set; }

		public JournalReport()
			: base(ReportType.ReportJournal)
		{
			ReportArchiveFilter = null;
		}

		public override ReportData GetData()
		{
			if (ReportArchiveFilter == null)
				ReportArchiveFilter = new ReportArchiveFilter();
			var data = new ReportData();
			data.ReportDocumentValues.Add("PrintDate", DateTime.Now);
			data.ReportDocumentValues.Add("StartDate", ReportArchiveFilter.StartDate);
			data.ReportDocumentValues.Add("EndDate", ReportArchiveFilter.EndDate);

			DataTable table = new DataTable("Journal");
			table.Columns.Add("DeviceTime");
			table.Columns.Add("SystemTime");
			table.Columns.Add("ZoneName");
			table.Columns.Add("Description");
			table.Columns.Add("DeviceName");
			table.Columns.Add("PanelName");
			table.Columns.Add("User");
			foreach (var journalRecord in ReportArchiveFilter.JournalRecords)
				table.Rows.Add(journalRecord.DeviceTime, journalRecord.SystemTime, journalRecord.ZoneName, journalRecord.Description, journalRecord.DeviceName, journalRecord.PanelName, journalRecord.User);
			data.DataTables.Add(table);
			return data;
		}

		public override bool IsFilterable
		{
			get { return true; }
		}
		public override void Filter(RelayCommand refreshCommand)
		{
			var archiveFilter = new ArchiveFilter()
			{
				EndDate = DateTime.Now,
				StartDate = DateTime.Now.AddDays(-1),
				UseSystemDate = false
			};
			var archiveFilterViewModel = new ArchiveFilterViewModel(archiveFilter);
			if (DialogService.ShowModalWindow(archiveFilterViewModel))
			{
				ReportArchiveFilter = new ReportArchiveFilter(archiveFilterViewModel);
				refreshCommand.Execute();
			}
		}
	}
	internal class ReportArchiveFilter
	{
		public ReportArchiveFilter()
		{
			SetFilter();
			Initialize();
		}

		public ReportArchiveFilter(ArchiveFilterViewModel archiveFilterViewModel)
		{
			SetFilter(archiveFilterViewModel);
			Initialize();
		}

		void Initialize()
		{
			JournalRecords = new List<JournalRecord>();
			LoadArchive();
		}

		public readonly DateTime ArchiveFirstDate = FiresecManager.FiresecService.GetArchiveStartDate().Result;
		public List<JournalRecord> JournalRecords { get; set; }
		public ArchiveFilter ArchiveFilter { get; set; }
		public bool IsFilterOn { get; set; }
		public DateTime StartDate { get; private set; }
		public DateTime EndDate { get; private set; }

		void SetFilter(ArchiveFilterViewModel archiveFilterViewModel)
		{
			ArchiveFilter = archiveFilterViewModel.GetModel();
			StartDate = archiveFilterViewModel.StartDate;
			EndDate = archiveFilterViewModel.EndDate;
		}

		void SetFilter()
		{
			var archiveFilter = new ArchiveFilter() { StartDate = ArchiveFirstDate, EndDate = DateTime.Now, UseSystemDate = false };
			var archiveFilterViewModel = new ArchiveFilterViewModel(archiveFilter);
			ArchiveFilter = archiveFilterViewModel.GetModel();
			StartDate = archiveFilterViewModel.StartDate;
			EndDate = archiveFilterViewModel.EndDate;
		}

		public void LoadArchive()
		{
			JournalRecords = new List<JournalRecord>();
			OperationResult<List<JournalRecord>> operationResult = FiresecManager.FiresecService.GetFilteredArchive(ArchiveFilter);
			if (operationResult.HasError == false)
			{
				foreach (var journalRecord in operationResult.Result)
				{
					JournalRecords.Add(journalRecord);
				}
			}
		}
	}
}
