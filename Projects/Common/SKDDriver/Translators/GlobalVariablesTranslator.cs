using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Automation;
using SKDDriver.DataAccess;

namespace SKDDriver.Translators
{
	public class GlobalVariablesTranslator
	{
		private readonly SKDDataContext _context;
		public GlobalVariablesTranslator(SKDDatabaseService databaseService)
		{
			_context = databaseService.Context;
		}

		public bool Save(Variable variable)
		{
			return true;
		}
	}
}
