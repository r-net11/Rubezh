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
			Name = ClauseToString(clauseOperationType, stateBit);
		}

		string ClauseToString(ClauseOperationType clauseOperationType, XStateBit stateBit)
		{
			switch (clauseOperationType)
			{
				case ClauseOperationType.AllZones:
				case ClauseOperationType.AnyZone:
					switch (stateBit)
					{
						case XStateBit.Fire1:
							return "Пожар 1";

						case XStateBit.Fire2:
							return "Пожар 2";

						case XStateBit.Attention:
							return "Внимание";
					}
					break;

				case ClauseOperationType.AllGuardZones:
				case ClauseOperationType.AnyGuardZone:
					switch (stateBit)
					{
						case XStateBit.On:
							return "Не на охране";

						case XStateBit.Off:
							return "На охране";

						case XStateBit.Attention:
							return "Сработка";
					}
					break;
			}

			return stateBit.ToDescription();
		}
	}
}