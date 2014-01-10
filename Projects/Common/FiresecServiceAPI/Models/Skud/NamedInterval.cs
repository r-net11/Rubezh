using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecAPI.Models.Skud
{
	public class NamedInterval
	{
		public Guid Uid { get; set; }
		public string Name { get; set; }
		public List<Interval> Intervals { get; set; }
	}
}
