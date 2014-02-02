using System;
using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using JournalModule.ViewModels;

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
			JournalRecords = new List<JournalRecordViewModel>();
		}

		public readonly DateTime ArchiveFirstDate = FiresecManager.GetArchiveStartDate().Result;
		public List<JournalRecordViewModel> JournalRecords { get; set; }
		public ArchiveFilter ArchiveFilter { get; set; }
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
			var archiveFilter = new ArchiveFilter() { StartDate = ArchiveFirstDate < DateTime.Now.AddDays(-1) ? DateTime.Now.AddDays(-1) : ArchiveFirstDate, EndDate = DateTime.Now, UseSystemDate = false };
			var archiveFilterViewModel = new ArchiveFilterViewModel(archiveFilter);
			ArchiveFilter = archiveFilterViewModel.GetModel();
			StartDate = archiveFilterViewModel.StartDateTime;
			EndDate = archiveFilterViewModel.EndDateTime;
		}

		public void LoadArchive()
		{
			JournalRecords = new List<JournalRecordViewModel>();
			//if (FiresecManager.IsFS2Enabled)
			//{
			//    var operationResult = FiresecManager.FS2ClientContract.GetFilteredArchive(ArchiveFilter);
			//    if (operationResult.HasError == false)
			//    {
			//        foreach (var journalRecord in operationResult.Result)
			//        {
			//            JournalRecords.Add(new JournalRecordViewModel(journalRecord));
			//        }
			//    }
			//}
			//else
			{
				var operationResult = FiresecManager.FiresecService.GetFilteredArchive(ArchiveFilter);
				if (operationResult.HasError == false)
				{
					foreach (var journalRecord in operationResult.Result)
					{
						JournalRecords.Add(new JournalRecordViewModel(journalRecord));
					}
				}
			}
		}
	}
}