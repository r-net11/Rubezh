using System;
using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecAPI.XModels;

namespace XFiresecAPI
{
	public class XDeviceState : XBaseState
	{
		public XDevice Device { get; private set; }

		public XDeviceState(XDevice device)
		{
			Device = device;
			if (device.Driver.DriverType != XDriverType.System)
				_isConnectionLost = true;
		}

		List<XStateType> _states = new List<XStateType>();
		public override List<XStateType> States
		{
			get
			{
				if (IsConnectionLost)
					return new List<XStateType>();

				if (!Device.IsRealDevice)
					return new List<XStateType>() { XStateType.Norm };

				if (Device.Driver.DriverType == XDriverType.PumpStation)
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
				if (Device.Parent != null && Device.Parent.Driver.IsGroupDevice)
				{
					Device.Parent.DeviceState.OnStateChanged();
				}
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

		public override List<XStateClass> StateClasses
		{
			get
			{
				var stateClasses = XStateClassHelper.Convert(States, IsConnectionLost, IsMissmatch);
				if (!IsConnectionLost && IsService)
				{
					stateClasses.Add(XStateClass.Service);
				}
				return stateClasses;
			}
		}

		public override XStateClass StateClass
		{
			get
			{
				if (Device.Driver.IsGroupDevice)
				{
					var childStateClasses = new List<XStateClass>();
					foreach (var child in Device.Children)
					{
						childStateClasses.AddRange(child.DeviceState.StateClasses);
					}
					return XStateClassHelper.GetMinStateClass(childStateClasses);
				}
				return XStateClassHelper.GetMinStateClass(StateClasses);
			}
		}

		public override StateType GetStateType()
		{
			if (!Device.IsRealDevice)
				return StateType.Norm;

			if (IsConnectionLost)
				return StateType.Unknown;
			else
				return XStatesHelper.XStateTypesToState(States);
		}
	}
}