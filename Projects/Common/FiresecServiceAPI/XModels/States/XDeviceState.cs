using System;
using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecAPI.XModels;

namespace XFiresecAPI
{
	public class XDeviceState
	{
		public XDevice Device { get; private set; }

		public XDeviceState(XDevice device)
		{
			_states = new List<XStateType>();
			Device = device;
			if (device.Driver.DriverType != XDriverType.System)
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

				if (!Device.IsRealDevice)
					return new List<XStateType>() { XStateType.Norm };

				if (_states == null)
					_states = new List<XStateType>();
				_states.Remove(XStateType.Save);
				return _states;
			}
			set
			{
				_states = value;
				if (_states == null)
					_states = new List<XStateType>();
				OnStateChanged();
			}
		}

		bool _isService;
		public bool IsService
		{
			get { return _isService; }
			set
			{
				if (_isService != value)
				{
					_isService = value;
					OnStateChanged();
				}
			}
		}

		public List<XStateClass> StateClasses
		{
			get
			{
				var stateClasses = XStateClassHelper.Convert(States, IsConnectionLost);
				if (!IsConnectionLost && IsService)
				{
					stateClasses.Add(XStateClass.Service);
				}
				return stateClasses;
			}
		}

		public XStateClass StateClass
		{
			get { return XStateClassHelper.GetMinStateClass(StateClasses); }
		}

		public StateType GetStateType()
		{
			if (!Device.IsRealDevice)
				return StateType.Norm;

			if (IsConnectionLost)
				return StateType.Unknown;
			else
				return XStatesHelper.XStateTypesToState(States);
		}

		public event Action StateChanged;
		void OnStateChanged()
		{
			if (StateChanged != null)
				StateChanged();
		}
	}
}