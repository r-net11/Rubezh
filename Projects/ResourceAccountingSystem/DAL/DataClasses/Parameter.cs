using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DataClasses
{
	public class Parameter
	{
		[Key]
		public Guid UID { get; set; }

		public Guid DeviceUID { get; set; }

		public Device Device { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public bool IsReadOnly { get; set; }

		public bool IsPollingEnabled { get; set; }

		public string Value { get; set; }
	}
}
