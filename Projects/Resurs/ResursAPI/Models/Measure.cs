using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResursAPI
{
	public class Measure
	{
		public Measure()
		{
			UID = Guid.NewGuid();
		}

		[Key]
		public Guid UID { get; set; }
		public Guid DeviceUID { get; set; }
		public Guid TariffUID { get; set; }
		public double Value { get; set; }
		public DateTime DateTime { get; set; }
	}
}
