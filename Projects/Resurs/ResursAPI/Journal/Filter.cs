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
			//StartDate = DateTime.Now.AddDays(-1);
			//EndDate = DateTime.Now;
			Count = 1;
			PageSize = 100;
			StateType = StateType.LastDays;
			JournalTypes = new List<JournalType>();
			ConsumerUIDs = new List<Guid?>();
			DeviceUIDs = new List<Guid?>();
			TariffUIDs = new List<Guid?>();
			UserUIDs = new List<Guid?>();
		}

		public DateTime StartDate { get; set;}
		public DateTime EndDate { get; set; }
		public List<JournalType> JournalTypes { get; set; }
		public int PageSize { get; set; }
		public int Count { get; set; }
		public StateType StateType { get; set; }
		public bool IsSortAsc { get; set; }
		public List<Guid?> ConsumerUIDs { get; set; }
		public List<Guid?> DeviceUIDs { get; set; }
		public List<Guid?> TariffUIDs { get; set; }
		public List<Guid?> UserUIDs { get; set; }
	}
}
