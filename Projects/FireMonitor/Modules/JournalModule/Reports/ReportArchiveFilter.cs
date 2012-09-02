using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JournalModule.ViewModels;
using FiresecAPI.Models;
using FiresecClient;
using FiresecAPI;

namespace JournalModule.Reports
{
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
			var archiveFilter = new ArchiveFilter() { StartDate = ArchiveFirstDate < DateTime.Now.AddDays(-1) ? DateTime.Now.AddDays(-1) : ArchiveFirstDate, EndDate = DateTime.Now, UseSystemDate = false };
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
