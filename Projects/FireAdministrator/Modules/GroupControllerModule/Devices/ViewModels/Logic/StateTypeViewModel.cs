using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using FiresecAPI.GK;

namespace GKModule.ViewModels
{
	public class StateTypeViewModel : BaseViewModel
	{
		public XStateBit StateBit { get; private set; }
		public string Name { get; private set; }

		public StateTypeViewModel(ClauseOperationType clauseOperationType, XStateBit stateBit)
		{
			StateBit = stateBit;
			Name = XClause.ClauseToString(clauseOperationType, stateBit);
		}
	}
}