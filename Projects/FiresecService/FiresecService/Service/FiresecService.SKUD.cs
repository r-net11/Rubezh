using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.Models.Skud;
using FiresecService.SKUD;

namespace FiresecService
{
	public partial class FiresecService : IFiresecService
	{
		private FiresecServiceSKUD _skud = new FiresecServiceSKUD();

		#region IFiresecService Members
		public IEnumerable<EmployeeCard> GetEmployees(EmployeeCardIndexFilter filter)
		{
			return _skud.GetEmployees(filter);
		}

		public bool DeleteEmployee(int id)
		{
			return _skud.DeleteEmployee(id);
		}

		public EmployeeCardDetails GetEmployeeCard(int id)
		{
			return _skud.GetEmployeeCard(id);
		}

		public int SaveEmployeeCard(EmployeeCardDetails employeeCard)
		{
			return _skud.SaveEmployeeCard(employeeCard);
		}

		public IEnumerable<EmployeeDepartment> GetEmployeeDepartments()
		{
			return _skud.GetEmployeeDepartments();
		}

		public IEnumerable<EmployeeGroup> GetEmployeeGroups()
		{
			return _skud.GetEmployeeGroups();
		}

		public IEnumerable<EmployeePosition> GetEmployeePositions()
		{
			return _skud.GetEmployeePositions();
		}
		#endregion
	}
}