using System;
using System.Collections.Generic;

namespace XFiresecAPI
{
	public class XDirectionState
	{
		public XDirection Direction { get; set; }

		public XDirectionState()
		{
			_states = new List<XStateType>();
			_isConnectionLost = true;
		}

		bool _isConnectionLost;
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

		public List<XStateClass> StateClasses
		{
			get { return XStateClassHelper.Convert(States, IsConnectionLost); }
		}

		public XStateClass StateClass
		{
			get { return XStateClassHelper.GetMinStateClass(StateClasses); }
		}

		public event Action StateChanged;
		void OnStateChanged()
		{
			if (StateChanged != null)
				StateChanged();
		}
	}
}