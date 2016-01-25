using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RubezhAPI.Journal
{
	public class JournalItemsCollection
	{
		public DateTime CreationDateTime { get; set; }
		public int? RecordCount { get; set; }
		public List<JournalItem> JournalItems { get; set; }
		public string GkIP { get; set; }
	}
}