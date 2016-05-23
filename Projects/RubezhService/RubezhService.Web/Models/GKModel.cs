using GKProcessor;
using RubezhAPI.GK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RubezhService.Models
{
	static class GKModel
	{
		static object locker = new object();
		static List<GKLifecycleItem> _gkLifecycleItems = new List<GKLifecycleItem>();


		public static void Initialize()
		{
			GKLifecycleManager.GKLifecycleChangedEvent += On_GKLifecycleChangedEvent;
		}

		public static List<GKLifecycleItem> GetLifecycleItems(int count)
		{
			lock (locker)
			{
				return _gkLifecycleItems.Skip(_gkLifecycleItems.Count - count).ToList();
			}
		}
		static void On_GKLifecycleChangedEvent(GKLifecycleInfo gkLifecycleInfo)
		{
			lock (locker)
			{
				var gkLifecycleItem = _gkLifecycleItems.FirstOrDefault(x => x.GKLifecycleInfo.UID == gkLifecycleInfo.UID);
				if (gkLifecycleItem == null)
					_gkLifecycleItems.Add(new GKLifecycleItem(gkLifecycleInfo));
				else
					gkLifecycleItem.Update();

				while (_gkLifecycleItems.Count > 50)
					_gkLifecycleItems.RemoveAt(0);
			}
            // TODO: Notify
        }
    }

	public class GKLifecycleItem
	{
		public GKLifecycleInfo GKLifecycleInfo { get; private set; }
		public DateTime DateTime { get; private set; }
		public string Address
		{
			get
			{
				if (GKLifecycleInfo.Device.DriverType == GKDriverType.GK)
					return GKLifecycleInfo.Device.PresentationAddress;
				else if (GKLifecycleInfo.Device.DriverType == GKDriverType.RSR2_KAU)
					return GKLifecycleInfo.Device.Parent.PresentationAddress + " КАУ " + GKLifecycleInfo.Device.PresentationAddress;
				return "";
			}
		}

		public GKLifecycleItem(GKLifecycleInfo gkLifecycleInfo)
		{
			GKLifecycleInfo = gkLifecycleInfo;
			Update();
		}

		public void Update()
		{
			DateTime = DateTime.Now;
		}
	}
}
