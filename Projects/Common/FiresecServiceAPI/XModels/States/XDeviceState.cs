﻿using System.Collections.Generic;
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

			if (device.DriverType == XDriverType.System)
				IsInitialState = false;

			MeasureParameter = new XMeasureParameter();
		}

		List<XStateBit> _stateBitss = new List<XStateBit>();
		public override List<XStateBit> StateBits
		{
			get
			{
				if (IsConnectionLost)
					return new List<XStateBit>();

				if (!Device.IsRealDevice)
					return new List<XStateBit>() { XStateBit.Norm };

				if (_stateBitss == null)
					_stateBitss = new List<XStateBit>();
				_stateBitss.Remove(XStateBit.Save);
				return _stateBitss;
			}
			set
			{
				_stateBitss = value;
				if (_stateBitss == null)
					_stateBitss = new List<XStateBit>();
				OnInternalStateChanged();
				if (Device.Parent != null && Device.Parent.Driver.IsGroupDevice)
				{
					Device.Parent.DeviceState.OnInternalStateChanged();
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
					OnInternalStateChanged();
				}
			}
		}

		public override List<XStateClass> InternalStateClasses
		{
			get
			{
				var stateClasses = base.InternalStateClasses;
				if (!IsConnectionLost && IsService)
				{
					stateClasses.Add(XStateClass.Service);
				}
				return stateClasses;
			}
		}

		public override XStateClass InternalStateClass
		{
			get
			{
				if (Device.Driver.IsGroupDevice)
				{
					var childStateClasses = new List<XStateClass>();
					foreach (var child in Device.Children)
					{
						childStateClasses.AddRange(child.DeviceState.InternalStateClasses);
					}
					return XStatesHelper.GetMinStateClass(childStateClasses);
				}
				return XStatesHelper.GetMinStateClass(InternalStateClasses);
			}
		}

		public XMeasureParameter MeasureParameter { get; set; }
	}
}