using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using System.Diagnostics;

namespace ClientFS2.ConfigurationWriter
{
	public class PanelDatabase
	{
		public PanelDatabase1 PanelDatabase1 { get; set; }
		public PanelDatabase2 PanelDatabase2 { get; set; }
		public Device ParentPanel { get; set; }

		static double Total_Miliseconds;

		public PanelDatabase(Device parentDevice)
		{
			var startDateTime = DateTime.Now;

			ParentPanel = parentDevice;
			PanelDatabase2 = new PanelDatabase2(parentDevice);
			PanelDatabase1 = new PanelDatabase1(PanelDatabase2);
			Trace.WriteLine("PanelDatabase Done");

			var deltaMiliseconds = (DateTime.Now - startDateTime).TotalMilliseconds;
			Total_Miliseconds += deltaMiliseconds;
			Trace.WriteLine("PanelDatabase Total_Miliseconds=" + Total_Miliseconds.ToString());
		}
	}
}