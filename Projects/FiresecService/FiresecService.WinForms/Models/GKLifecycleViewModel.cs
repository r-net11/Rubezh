using RubezhAPI.GK;
using GKProcessor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace FiresecService.ViewModels
{
	public class GKLifecycle
	{
		public GKLifecycle(GKLifecycleInfo gkLifecycleInfo)
		{
			GKLifecycleInfo = gkLifecycleInfo;
			Update(gkLifecycleInfo);
		}
		public GKLifecycleInfo GKLifecycleInfo { get; private set; }
		public void Update(GKLifecycleInfo gkLifecycleInfo)
		{
			Time = System.DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss");
			if (gkLifecycleInfo.Device.DriverType == GKDriverType.GK)
			{
				Address = gkLifecycleInfo.Device.PresentationAddress;
			}
			else if (gkLifecycleInfo.Device.DriverType == GKDriverType.RSR2_KAU)
			{
				Address = gkLifecycleInfo.Device.Parent.PresentationAddress + " КАУ " + gkLifecycleInfo.Device.PresentationAddress;
			}
			Name = gkLifecycleInfo.Name;
			Progress = gkLifecycleInfo.Progress;
			Items = gkLifecycleInfo.DetalisationItems;
		}
		public string Address { get; set; }
		public string Name { get; set; }
		public string Progress { get; set; }
		public string Time { get; set; }
		public List<string> Items { get; set; }
	}
}