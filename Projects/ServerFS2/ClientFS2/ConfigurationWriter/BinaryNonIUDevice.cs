using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ClientFS2.ConfigurationWriter
{
	public class BinaryNonIUDevice
	{
		BytesDatabase BytesDatabase;
		Device PanelDevice;
		Device Device;

		public BinaryNonIUDevice(Device panelDevice, Device device)
		{
			PanelDevice = panelDevice;
			Device = device;
			BytesDatabase = new BytesDatabase();
		}
	}
}