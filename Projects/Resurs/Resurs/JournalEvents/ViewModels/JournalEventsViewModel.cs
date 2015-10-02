using Infrastructure.Common;
using ResursAPI;
using ResursDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class JournalEventsViewModel
	{
		public JournalEventsViewModel()
		{
			Events = new List<JournalEventViewModel>();
			foreach (var JournalItem in DBCash.GetJournal())
			{
				Events.Add(new JournalEventViewModel(JournalItem));
			}
		}

		public List<JournalEventViewModel> Events{ get; set; }
		public bool IsVisibility
		{
			get { return DBCash.CurrentUser.UserPermissions.Any(x => x.PermissionType == PermissionType.Journal); }
		}

	}
}
