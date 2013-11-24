using System;
using System.Collections.Generic;
using GKProcessor;
using FiresecClient;
using GKModule.ViewModels;
using XFiresecAPI;

namespace GKModule.Reports
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
			JournalItems = new List<JournalItem>();
		}

		public readonly DateTime ArchiveFirstDate = FiresecManager.GetArchiveStartDate().Result;
		public List<JournalItem> JournalItems { get; set; }
		public XArchiveFilter ArchiveFilter { get; set; }
		public bool IsFilterOn { get; set; }
		public DateTime StartDate { get; private set; }
		public DateTime EndDate { get; private set; }

		void SetFilter(ArchiveFilterViewModel archiveFilterViewModel)
		{
			ArchiveFilter = archiveFilterViewModel.GetModel();
			StartDate = archiveFilterViewModel.StartDateTime;
			EndDate = archiveFilterViewModel.EndDateTime;
		}

		void SetFilter()
		{
			var archiveFilter = new XArchiveFilter() { StartDate = ArchiveFirstDate < DateTime.Now.AddHours(-1) ? DateTime.Now.AddHours(-1) : ArchiveFirstDate, EndDate = DateTime.Now };
			var archiveFilterViewModel = new ArchiveFilterViewModel(archiveFilter);
			ArchiveFilter = archiveFilterViewModel.GetModel();
			StartDate = archiveFilterViewModel.StartDateTime;
			EndDate = archiveFilterViewModel.EndDateTime;
		}

		public void LoadArchive()
		{
			var filteredJournalItems = GKDBHelper.Select(ArchiveFilter);
			foreach (var journalItem in filteredJournalItems)
			{
				JournalItems.Add(journalItem);
			}
		}
	}
}