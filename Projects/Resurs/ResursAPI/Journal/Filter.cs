using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public class Filter
	{
		public Filter()
		{
			StartDate = DateTime.Now.AddDays(-1);
			EndDate = DateTime.Now;
			JournalTypes = new List<JournalType>();
			Count = 1;
			PageSize = 100;
			StateType = StateType.LastDays;
		}

		public DateTime StartDate { get; set;}
		public DateTime EndDate { get; set; }
		public List<JournalType> JournalTypes { get; set; }
		public int PageSize { get; set; }
		public int Count { get; set; }
		public StateType StateType { get; set; }
		public bool IsSortAsc { get; set; }
	}
}
