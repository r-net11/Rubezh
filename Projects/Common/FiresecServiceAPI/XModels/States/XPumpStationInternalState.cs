using System.Collections.Generic;

namespace XFiresecAPI
{
	public class XPumpStationInternalState : XBaseInternalState
	{
		public XPumpStation PumpStation { get; set; }

		public XPumpStationInternalState(XPumpStation pumpStation)
		{
			PumpStation = pumpStation;
		}
	}
}