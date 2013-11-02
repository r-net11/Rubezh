using System.Collections.Generic;

namespace XFiresecAPI
{
	public class XDelayState : XBaseState
	{
		public XDelay Delay { get; set; }

		public XDelayState()
		{
		}

		List<XStateBit> _stateBits = new List<XStateBit>();
		public override List<XStateBit> StateBits
		{
			get
			{
				if (IsConnectionLost)
					return new List<XStateBit>();
				else
				{
					if (_stateBits == null)
						_stateBits = new List<XStateBit>();
					return _stateBits;
				}
			}
			set
			{
				_stateBits = value;
				if (_stateBits == null)
					_stateBits = new List<XStateBit>();
				OnStateChanged();
			}
		}
	}
}