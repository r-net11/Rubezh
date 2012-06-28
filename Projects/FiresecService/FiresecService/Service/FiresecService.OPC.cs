using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecService.OPC;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		public void OPCRefresh(DeviceConfiguration deviceConfiguration)
		{
			FiresecOPCManager.OPCRefresh(deviceConfiguration);
		}

		public void OPCRegister()
		{
			FiresecOPCManager.OPCRegister();
		}

		public void OPCUnRegister()
		{
			FiresecOPCManager.OPCUnRegister();
		}
	}
}