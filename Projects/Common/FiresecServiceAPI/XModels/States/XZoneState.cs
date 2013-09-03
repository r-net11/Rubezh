using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.XModels;

namespace XFiresecAPI
{
	public class XZoneState : XBaseState
	{
		public XZone Zone { get; set; }

		public XZoneState()
		{
			_isConnectionLost = true;
		}

		List<XStateBit> _states = new List<XStateBit>();
		public override List<XStateBit> StateBits
		{
			get
			{
				if (IsConnectionLost)
					return new List<XStateBit>();
				else
				{
					if (_states == null)
						_states = new List<XStateBit>();
					return _states;
				}
			}
			set
			{
				_states = value;
				if (_states == null)
					_states = new List<XStateBit>();
				OnStateChanged();
			}
		}
	}
}