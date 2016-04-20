using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.GK;

namespace GKModule.ViewModels
{
	public class StateTypeViewModel : BaseViewModel
	{
		public GKStateBit StateBit { get; private set; }
		public string Name { get; private set; }

		public StateTypeViewModel(ClauseOperationType clauseOperationType, GKStateBit stateBit)
		{
			StateBit = stateBit;
			Name = GKClause.ClauseToString(clauseOperationType, stateBit);
		}
	}
}