using System.Collections.Generic;
using FiresecAPI.Models.Skud;

namespace FiresecClient
{
	public partial class FiresecManager
	{
		public static IEnumerable<EmployeeCard> GetEmployees(EmployeeCardIndexFilter filter)
		{
			return FiresecService.GetEmployees(filter);
		}
		public static bool DeleteEmployeeCard(EmployeeCard card)
		{
			return FiresecService.DeleteEmployee(card.Id);
		}
		public static EmployeeCardDetails GetEmployeeCard(EmployeeCard card)
		{
			return FiresecService.GetEmployeeCard(card.Id);
		}
		public static bool SaveEmployeeCard(EmployeeCardDetails card)
		{
			int id = FiresecService.SaveEmployeeCard(card);
			if (id != -1)
				card.Id = id;
			return id != -1;
		}

		public static IEnumerable<EmployeeDepartment> GetEmployeeDepartments()
		{
			return FiresecService.GetEmployeeDepartments();
		}
		public static IEnumerable<EmployeeGroup> GetEmployeeGroups()
		{
			return FiresecService.GetEmployeeGroups();
		}
		public static IEnumerable<EmployeePosition> GetEmployeePositions()
		{
			return FiresecService.GetEmployeePositions();
		}
	}
}