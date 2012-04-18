using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models.Skud;

namespace FiresecClient
{
	public partial class FiresecManager
	{
		public static IEnumerable<EmployeeCardIndex> GetEmployees(EmployeeCardIndexFilter filter)
		{
			return FiresecService.GetEmployees(filter);
		}
		public static bool DeleteEmployeeCard(EmployeeCardIndex card)
		{
			return FiresecService.DeleteEmployee(card.Id);
		}
		public static EmployeeCard GetEmployeeCard(EmployeeCardIndex card)
		{
			return FiresecService.GetEmployeeCard(card.Id);
		}
		public static bool SaveEmployeeCard(EmployeeCard card)
		{
			int id = FiresecService.SaveEmployeeCard(card);
			if (id != -1)
				card.Id = id;
			return id != -1;
		}
	}
}