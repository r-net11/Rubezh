using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecAPI.Models.Skud
{
	public class Holiday
	{
		public Guid Uid { get; set; }
		public string Name { get; set; }
		public HolidayType Type { get; set; }
		public DateTime Date { get; set; }
		public DateTime TransferDate { get; set; }
		public int Reduction { get; set; }
	}

	public enum HolidayType
	{
		Holiday,
		Reduced,
		Transferred,
		Working
	}
}
