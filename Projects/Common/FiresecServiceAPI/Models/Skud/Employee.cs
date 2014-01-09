using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecAPI.Models.Skud
{
	public class Employee
	{
		public Guid Uid { get; set; }
		public string FirstName { get; set; }
		public string SecondName { get; set; }
		public string LastName { get; set; }
		public Position Position { get; set; }
		public Department Department { get; set; }
		public EmployeeReplacement Replacement { get; set; }
		public Schedule Schedule { get; set; }
		public List<AdditionalColumn> AdditionalColumns { get; set; }
		public DateTime Appointed { get; set; }
		public DateTime Dismissed { get; set; }
	}
}
