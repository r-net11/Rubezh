using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKDDriver.DataClasses
{
	public class OrganisationHRCash
	{
		public OrganisationHRCash()
		{
			Employees = new List<Employee>();
			Departments = new List<Department>();
			Positions = new List<Position>();
		}

		public Guid OrganisationUID;
		public List<Employee> Employees;
		public List<Department> Departments;
		public List<Position> Positions;
	}
}
