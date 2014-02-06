using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;

namespace XFiresecAPI
{
	public class XDeviceInternalState : XBaseInternalState
	{
		public XDevice Device { get; private set; }

		public XDeviceInternalState(XDevice device)
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
			}
		}

		public override List<XStateClass> StateClasses
		{
			get
			{
				var stateClasses = base.StateClasses;
				if (!IsConnectionLost && IsService)
				{
					if (!stateClasses.Contains(XStateClass.Service))
						stateClasses.Add(XStateClass.Service);
				}
				if (Device.Driver.IsGroupDevice || Device.DriverType == XDriverType.KAU_Shleif || Device.DriverType == XDriverType.RSR2_KAU_Shleif)
				{
					stateClasses.Clear();
					if (Device.Children.Count > 0)
					{
						foreach (var child in Device.Children)
						{
							if (!stateClasses.Contains(child.InternalState.StateClass))
								stateClasses.Add(child.InternalState.StateClass);
						}
						if (stateClasses.Count > 1 && stateClasses.Contains(XStateClass.Norm))
							stateClasses.Remove(XStateClass.Norm);
					}
				}
				stateClasses.OrderBy(x => x);
				return stateClasses;
			}
		}
	}
}