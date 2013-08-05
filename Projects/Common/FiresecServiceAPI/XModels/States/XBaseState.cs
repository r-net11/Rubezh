using System;
using System.Collections.Generic;
using System.Windows;
using FiresecAPI;

namespace XFiresecAPI
{
	public abstract class XBaseState
	{
		public static void SafeCall(Action action)
		{
			if (Application.Current != null && Application.Current.Dispatcher != null)
				Application.Current.Dispatcher.BeginInvoke(action);
		}

		public XBaseState()
		{
			AdditionalStates = new List<string>();
			AdditionalStateProperties = new List<AdditionalXStateProperty>();
		}

		public event Action StateChanged;
		public void OnStateChanged()
		{
			SafeCall(() =>
			{
				if (StateChanged != null)
					StateChanged();
			});
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
		public DateTime LastDateTime { get; set; }

		List<string> _additionalStates;
		public List<string> AdditionalStates
		{
			get { return _additionalStates; }
			set
			{
				_additionalStates = value;
				OnStateChanged();
			}
		}

		List<AdditionalXStateProperty> _additionalStateProperties;
		public List<AdditionalXStateProperty> AdditionalStateProperties
		{
			get { return _additionalStateProperties; }
			set
			{
				_additionalStateProperties = value;
				OnStateChanged();
			}
		}

		int _onDelay;
		public int OnDelay
		{
			get { return _onDelay; }
			set
			{
				_onDelay = value;
				OnStateChanged();
			}
		}

		int _holdDelay;
		public int HoldDelay
		{
			get { return _holdDelay; }
			set
			{
				_holdDelay = value;
				OnStateChanged();
			}
		}

		int _offDelay;
		public int OffDelay
		{
			get { return _offDelay; }
			set
			{
				_offDelay = value;
				OnStateChanged();
			}
		}
	}
}