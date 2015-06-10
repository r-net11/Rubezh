using System;
using System.ComponentModel.DataAnnotations;

namespace SKDDriver.DataClasses
{
	public class CurrentConsumption
	{
		[Key]
		public Guid UID { get; set; }

		public Guid AlsUID { get; set; }

		public int Current { get; set; }

		public DateTime DateTime { get; set; }
	}
}
