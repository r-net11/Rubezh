using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.XModels;

namespace XFiresecAPI
{
	public class XDelayState : XBaseState
	{
		public XDelay Delay { get; set; }

		public XDelayState()
		{
			_isConnectionLost = true;
		}

		List<XStateType> _states = new List<XStateType>();
		public override List<XStateType> States
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

		public override List<XStateClass> StateClasses
		{
			get { return XStateClassHelper.Convert(States, IsConnectionLost, IsMissmatch); }
		}

		public override XStateClass StateClass
		{
			get { return XStateClassHelper.GetMinStateClass(StateClasses); }
		}

		public override StateType GetStateType()
		{
			if (IsConnectionLost)
				return StateType.Unknown;
			else
				return XStatesHelper.XStateTypesToState(States);
		}
	}
}