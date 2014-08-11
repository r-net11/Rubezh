using System;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI.EmployeeTimeIntervals;
using LinqKit;
using OperationResult = FiresecAPI.OperationResult;
using FiresecAPI.SKD;
using FiresecAPI;

namespace SKDDriver.Translators
{
	public class TimeTrackTranslator
	{
		DataAccess.SKDDataContext Context;

		public TimeTrackTranslator(DataAccess.SKDDataContext context)
		{
			Context = context;
		}
	}
}