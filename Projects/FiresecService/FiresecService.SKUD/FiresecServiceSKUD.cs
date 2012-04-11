using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using FiresecAPI.Models.Skud;

namespace FiresecService.SKUD
{
	public class FiresecServiceSKUD : IFiresecServiceSKUD
	{
		#region IFiresecServiceSKUD Members

		public IEnumerable<SkudEmployee> GetEmployees()
		{
			return null;
		}

		#endregion
	}
}
