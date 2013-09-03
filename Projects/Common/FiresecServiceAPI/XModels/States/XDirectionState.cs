﻿using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.XModels;

namespace XFiresecAPI
{
	public class XDirectionState : XBaseState
	{
		public XDirection Direction { get; set; }

		public XDirectionState()
		{
			_isConnectionLost = true;
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