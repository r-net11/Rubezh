using System.Collections.Generic;
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
					Device.Parent.InternalState.OnInternalStateChanged();
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

		public override List<XStateClass> StateClasses
		{
			get
			{
				var stateClasses = base.StateClasses;
				if (!IsConnectionLost && IsService)
				{
					stateClasses.Add(XStateClass.Service);
					if (stateClasses.Contains(XStateClass.Service))
						stateClasses.Remove(XStateClass.Service);
				}
				return stateClasses;
			}
		}

		public override XStateClass StateClass
		{
			get
			{
				if (Device.Driver.IsGroupDevice || Device.DriverType == XDriverType.KAU_Shleif || Device.DriverType == XDriverType.RSR2_KAU_Shleif)
				{
					var childStateClasses = new List<XStateClass>();
					foreach (var child in Device.Children)
					{
						childStateClasses.Add(child.InternalState.StateClass);
					}
					return XStatesHelper.GetMinStateClass(childStateClasses);
				}
				return XStatesHelper.GetMinStateClass(StateClasses);
			}
		}
	}
}