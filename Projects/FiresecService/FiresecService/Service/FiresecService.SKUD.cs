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


		public IEnumerable<EmployeeCardIndex> GetEmployees()
		{
			return _skud.GetEmployees();
		}

		#endregion
	}
}