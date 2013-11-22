using System.Collections.Generic;

namespace XFiresecAPI
{
	public class XPimState : XBaseState
	{
		public XPim Pim { get; set; }

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