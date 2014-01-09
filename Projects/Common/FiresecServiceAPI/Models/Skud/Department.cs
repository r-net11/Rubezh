using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecAPI.Models.Skud
{
	public class Department
	{
		public Guid Uid { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public Department ParentDepartment { get; set; }
		public Employee ContactEmployee { get; set; }
		public Employee AttendantEmployee { get; set; }
		public List<Phone> Phones { get; set; }
 	}
}
