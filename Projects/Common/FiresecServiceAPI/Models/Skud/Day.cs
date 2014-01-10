using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecAPI.Models.Skud
{
	public class Day
	{
		public Guid Uid { get; set; }
		public NamedInterval NamedInterval { get; set; }
		public int? Number { get; set; }
	}
}
