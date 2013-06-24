using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using System.Diagnostics;

namespace ServerFS2.Monitoring
{
	public static class NonPanelStatesManager
	{
		public static void UpdatePDUPanelState(Device panel, bool isSilent = false)
		{
			var bytes = ServerHelper.GetDeviceStatus(panel);
			Trace.WriteLine(panel.PresentationAddressAndName + " " + BytesHelper.BytesToString(bytes));
		}
	}
}