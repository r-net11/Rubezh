using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecAPI.Models.Skud
{
	public class EmployeeReplacement
	{
		public Guid Uid { get; set; }
		public DateTime BeginDate { get; set; }
		public DateTime EndDate { get; set; }
		public Department Department { get; set; }
		public Schedule Schedule { get; set; }
	}
}
