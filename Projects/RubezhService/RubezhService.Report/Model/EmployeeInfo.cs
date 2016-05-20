using System;
using RubezhAPI.SKD;

namespace RubezhService.Report.Model
{
	public class EmployeeInfo : OrganisationBaseObjectInfo<Employee>
	{
		public Guid? DepartmentUID { get; set; }
		public string Department { get; set; }
		public Guid? PositionUID { get; set; }
		public string Position { get; set; }
	}
}
