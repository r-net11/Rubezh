using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DataClasses
{
	public class Measure
	{
		[Key]
		public Guid UID { get; set; }
		public Guid DeviceUID { get; set; }
		public Guid TariffUID { get; set; }
		public double Value { get; set; }
		public DateTime DateTime { get; set; }
	}
}
