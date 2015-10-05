using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class JournalEventViewModel
	{
		public JournalEventViewModel(Journal journal)
		{
			Journal = journal;
		}
		public Journal Journal { get; set; }
	}
}
