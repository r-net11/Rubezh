using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;

namespace XFiresecAPI
{
	public abstract class XBaseState
	{
		public event Action StateChanged;
		protected void OnStateChanged()
		{
			if (StateChanged != null)
				StateChanged();
		}

		protected bool _isConnectionLost;
		public bool IsConnectionLost
		{
			get { return _isConnectionLost; }
			set
			{
				if (_isConnectionLost != value)
				{
					_isConnectionLost = value;
					OnStateChanged();
				}
			}
		}

		bool _isMissmatch;
		public bool IsMissmatch
		{
			get { return _isMissmatch; }
			set
			{
				if (_isMissmatch != value)
				{
					_isMissmatch = value;
					OnStateChanged();
				}
			}
		}

		public abstract List<XStateType> States { get; set; }
		public abstract List<XStateClass> StateClasses { get; }
		public abstract XStateClass StateClass { get; }
		public abstract StateType GetStateType();
	}
}