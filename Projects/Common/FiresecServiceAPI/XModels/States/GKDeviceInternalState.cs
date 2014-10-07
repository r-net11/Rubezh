using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;

namespace FiresecAPI.GK
{
	public class GKDeviceInternalState : GKBaseInternalState
	{
		public GKDevice Device { get; private set; }

		public GKDeviceInternalState(GKDevice device)
		{
			Device = device;

			if (device.DriverType == GKDriverType.System)
				IsInitialState = false;
		}

		List<GKStateBit> _stateBitss = new List<GKStateBit>();
		public override List<GKStateBit> StateBits
		{
			get
			{
				if (IsConnectionLost)
					return new List<GKStateBit>();

				if (!Device.IsRealDevice)
					return new List<GKStateBit>() { GKStateBit.Norm };

				if (_stateBitss == null)
					_stateBitss = new List<GKStateBit>();
				_stateBitss.Remove(GKStateBit.Save);
				return _stateBitss;
			}
			set
			{
				_stateBitss = value;
				if (_stateBitss == null)
					_stateBitss = new List<GKStateBit>();
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
				if (Device.Driver.IsGroupDevice || Device.DriverType == GKDriverType.KAU_Shleif || Device.DriverType == GKDriverType.RSR2_KAU_Shleif)
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