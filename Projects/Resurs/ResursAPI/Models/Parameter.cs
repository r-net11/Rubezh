using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResursAPI
{
	public class Parameter
	{
		public Parameter()
		{
			UID = Guid.NewGuid();
		}
		[Key]
		public Guid UID { get; set; }
		public Device Device { get; set; }
		[NotMapped]
		public ResursAPI.DriverParameter DriverParameter { get; set; }
		public bool IsPollingEnabled { get; set; }
		public int Number { get; set; }
		public int? IntValue { get; set; }
		public double? DoubleValue { get; set; }
		public bool BoolValue { get; set; }
		[MaxLength(4000)]
		public string StringValue { get; set; }
		public DateTime? DateTimeValue { get; set; }
	}
}
