using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ClientFS2.ConfigurationWriter
{
	public class PanelDatabase
	{
		public PanelDatabase1 PanelDatabase1 { get; set; }
		public PanelDatabase2 PanelDatabase2 { get; set; }
		public Device ParentPanel { get; set; }

		public PanelDatabase(Device parentDevice)
		{
			ParentPanel = parentDevice;
			PanelDatabase2 = new PanelDatabase2(parentDevice);
			PanelDatabase1 = new PanelDatabase1(PanelDatabase2);
		}
	}
}