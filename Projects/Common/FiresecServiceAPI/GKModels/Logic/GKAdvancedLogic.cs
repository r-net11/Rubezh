using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecAPI.GK
{
	public class GKAdvancedLogic : GKLogic
	{
		public bool HasOnClause { get; private set; }
		public bool HasOnNowClause { get; private set; }
		public bool HasOffClause { get; private set; }
		public bool HasOffNowClause { get; private set; }
		public bool HasStopClause { get; private set; }

		public GKAdvancedLogic(bool hasOnClause, bool hasOnNowClause, bool hasOffClause, bool hasOffNowClause, bool hasStopClause)
		{
			HasOnClause = hasOnClause;
			HasOnNowClause = hasOnNowClause;
			HasOffClause = hasOffClause;
			HasOffNowClause = hasOffNowClause;
			HasStopClause = hasStopClause;
		}
	}
}
