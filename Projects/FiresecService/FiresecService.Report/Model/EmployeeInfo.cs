using StrazhAPI.SKD;
using System;

namespace FiresecService.Report.Model
{
	public class EmployeeInfo : OrganisationBaseObjectInfo<Employee>
	{
		public Guid? DepartmentUID { get; set; }

		public string Department { get; set; }

		public Guid? PositionUID { get; set; }

		public string Position { get; set; }
	}
}