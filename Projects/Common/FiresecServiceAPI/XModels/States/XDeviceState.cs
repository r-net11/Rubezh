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

			MeasureParameter = new XMeasureParameter();
		}

		List<XStateBit> _states = new List<XStateBit>();
		public override List<XStateBit> StateBits
		{
			get
			{
				if (IsConnectionLost)
					return new List<XStateBit>();

				if (!Device.IsRealDevice)
					return new List<XStateBit>() { XStateBit.Norm };

				if (Device.Driver.DriverType == XDriverType.PumpStation)
					return new List<XStateBit>() { XStateBit.Norm };

				if (_states == null)
					_states = new List<XStateBit>();
				_states.Remove(XStateBit.Save);
				return _states;
			}
			set
			{
				_states = value;
				if (_states == null)
					_states = new List<XStateBit>();
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
				var stateClasses = XStatesHelper.StateBitsToStateClasses(StateBits, IsConnectionLost, IsGKMissmatch, IsInTechnologicalRegime, IsNoLicense);
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
					return XStatesHelper.GetMinStateClass(childStateClasses);
				}
				return XStatesHelper.GetMinStateClass(StateClasses);
			}
		}

		public XMeasureParameter MeasureParameter { get; set; }
	}
}