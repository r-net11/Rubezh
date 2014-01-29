using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;

namespace SKDDriver
{
	public static class SKDProcessorManager
	{
		public static void OnGKCallbackResult(SKDCallbackResult gkCallbackResult)
		{
			if (gkCallbackResult.JournalItems.Count +
				gkCallbackResult.GKStates.DeviceStates.Count > 0)
			{
				if (SKDCallbackResultEvent != null)
					SKDCallbackResultEvent(gkCallbackResult);
			}
		}
		public static event Action<SKDCallbackResult> SKDCallbackResultEvent;
	}
}