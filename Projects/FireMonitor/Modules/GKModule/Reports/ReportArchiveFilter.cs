using System;
using System.Collections.Generic;
using FiresecAPI.GK;
using GKModule.ViewModels;
using GKProcessor;

namespace GKModule.Reports
{
	internal class ReportArchiveFilter
	{
		public ReportArchiveFilter()
		{
			ArchiveFilter = new GKArchiveFilter() { StartDate = ArchiveFirstDate < DateTime.Now.AddSeconds(-1) ? DateTime.Now.AddSeconds(-1) : ArchiveFirstDate, EndDate = DateTime.Now };
			StartDate = ArchiveFilter.StartDate;
			EndDate = ArchiveFilter.EndDate;
			Initialize();
		}

		public ReportArchiveFilter(ArchiveFilterViewModel archiveFilterViewModel)
		{
			SetFilter(archiveFilterViewModel);
			Initialize();
		}

		void Initialize()
		{
			JournalItems = new List<GKJournalItem>();
		}

		public readonly DateTime ArchiveFirstDate;// = FiresecManager.GetArchiveStartDate().Result;
		public List<GKJournalItem> JournalItems { get; set; }
		public GKArchiveFilter ArchiveFilter { get; set; }
		public bool IsFilterOn { get; set; }
		public DateTime StartDate { get; private set; }
		public DateTime EndDate { get; private set; }

		void SetFilter(ArchiveFilterViewModel archiveFilterViewModel)
		{
			ArchiveFilter = archiveFilterViewModel.GetModel();
			StartDate = archiveFilterViewModel.StartDateTime.DateTime;
			EndDate = archiveFilterViewModel.EndDateTime.DateTime;
		}

		public void LoadArchive()
		{
			var filteredJournalItems = GKDBHelper.BeginGetGKFilteredArchive(ArchiveFilter, Guid.NewGuid(), true);
			foreach (var journalItem in filteredJournalItems)
			{
				JournalItems.Add(journalItem);
			}
		}
	}
}