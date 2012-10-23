using System;
using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecAPI.XModels;

namespace XFiresecAPI
{
    public class XDeviceState
    {
		public XDevice Device { get; set; }

        public XDeviceState()
        {
			_states = new List<XStateType>();
			_isConnectionLost = true;
        }

		bool _isConnectionLost;
		public bool IsConnectionLost
		{
			get
			{
				if (!Device.IsRealDevice)
					return false;

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
				if (!Device.IsRealDevice)
					return new List<XStateType>();

				if (IsConnectionLost)
					return new List<XStateType>();
				else
				{
					if (_states == null)
						_states = new List<XStateType>();
					_states.Remove(XStateType.Save);
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
				if (!Device.IsRealDevice)
					return StateType.Norm;

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