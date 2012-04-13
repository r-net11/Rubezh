using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using FiresecAPI.Models.Skud;
using FiresecService.SKUD.Translator;

namespace FiresecService.SKUD
{
	public class FiresecServiceSKUD : IFiresecServiceSKUD
	{
		#region IFiresecServiceSKUD Members

		public IEnumerable<SkudEmployee> GetEmployees()
		{
			DataAccess.FiresecDataContext context = new DataAccess.FiresecDataContext();
			return EmployeeResultTranslator.Translate(context.GetAllEmployees());
		}

		#endregion
	}
}
