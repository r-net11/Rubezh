using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;
using FiresecAPI;
using FiresecAPI.XModels;

namespace XFiresecAPI
{
	public class XDirectionState
	{
		public XDirectionState()
		{
			_states = new List<XStateType>();
			_isConnectionLost = true;
		}

		public XDirection Direction { get; set; }
		public Guid UID { get; set; }

		bool _isConnectionLost;
		public bool IsConnectionLost
		{
			get
			{
				return _isConnectionLost;
			}
			set
			{
				if (_isConnectionLost != value)
				{
					_isConnectionLost = value;
					OnStateChanged();
				}
			}
		}

		List<XStateType> _states;
		public List<XStateType> States
		{
			get
			{
				if (IsConnectionLost)
					return new List<XStateType>();
				else
				{
					if (_states == null)
						_states = new List<XStateType>();
					return _states;
				}
			}
			set
			{
				_states = value;
				if (_states == null)
					_states = new List<XStateType>();
				OnStateChanged();
			}
		}

		public StateType StateType
		{
			get
			{
				if (IsConnectionLost)
					return StateType.Unknown;
				else
					return XStatesHelper.XStateTypesToState(States);
			}
		}

		public event Action StateChanged;
		void OnStateChanged()
		{
			if (StateChanged != null)
				StateChanged();
		}
	}
}