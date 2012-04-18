using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FiresecAPI.Models;
using FiresecService.Converters;
using Firesec;
using FiresecAPI;
using FiresecService.SKUD;
using FiresecAPI.Models.Skud;

namespace FiresecService
{
	public partial class FiresecService : IFiresecService
	{
		private FiresecServiceSKUD _skud = new FiresecServiceSKUD();


		#region IFiresecService Members


		public IEnumerable<EmployeeCardIndex> GetEmployees(EmployeeCardIndexFilter filter)
		{
			return _skud.GetEmployees(filter);
		}

		public bool DeleteEmployee(int id)
		{
			return _skud.DeleteEmployee(id);
		}

		public EmployeeCard GetEmployeeCard(int id)
		{
			return _skud.GetEmployeeCard(id);
		}

		public int SaveEmployeeCard(EmployeeCard employeeCard)
		{
			return _skud.SaveEmployeeCard(employeeCard);
		}

		#endregion
	}
}