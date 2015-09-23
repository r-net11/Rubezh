using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResursDAL.DataClasses
{
	public class Parameter:DbModelBase
	{
		public Parameter():base() {}
		
		public Guid DeviceUID { get; set; }

		public Device Device { get; set; }

		public bool IsReadOnly { get; set; }

		public bool IsPollingEnabled { get; set; }

		public string Value { get; set; }
	}
}
