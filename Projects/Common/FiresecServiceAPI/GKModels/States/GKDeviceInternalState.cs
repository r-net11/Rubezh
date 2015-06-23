using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;

namespace FiresecAPI.GK
{
	public class GKDeviceInternalState : GKBaseInternalState
	{
		public GKDevice Device { get; private set; }

		public GKDeviceInternalState(GKDevice device)
		{
			Device = device;
		}

		List<GKStateBit> _stateBitss = new List<GKStateBit>();
		public override List<GKStateBit> StateBits
		{
			get
			{
				if (IsConnectionLost)
					return new List<GKStateBit>();

				if (_stateBitss == null)
					_stateBitss = new List<GKStateBit>();
				_stateBitss.Remove(GKStateBit.Save);
				return _stateBitss;
			}
			set
			{
				_stateBitss = value;
				if (_stateBitss == null)
					_stateBitss = new List<GKStateBit>();
			}
		}

		public override List<XStateClass> StateClasses
		{
			get
			{
				var stateClasses = base.StateClasses;
				if (!IsConnectionLost && IsService)
				{
					if (!stateClasses.Contains(XStateClass.Service))
						stateClasses.Add(XStateClass.Service);
				}

				stateClasses.OrderBy(x => x);
				return stateClasses;
			}
		}
	}
}