using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FiresecAPI;
using FiresecClient;
using Infrastructure;
using SKDModule.Events;
using SKDModule.ViewModels;
using XFiresecAPI;

namespace SKDModule
{
	public static class AutoActivationWatcher
	{
		public static void Run()
		{
			ServiceFactory.Events.GetEvent<NewSKDJournalEvent>().Unsubscribe(OnNewJournal);
			ServiceFactory.Events.GetEvent<NewSKDJournalEvent>().Subscribe(OnNewJournal);
		}

		public static void OnNewJournal(List<SKDJournalItem> journalItems)
		{
			if (ClientSettings.AutoActivationSettings.IsAutoActivation)
			{
				if ((Application.Current.MainWindow != null) && (!Application.Current.MainWindow.IsActive))
				{
					Application.Current.MainWindow.WindowState = System.Windows.WindowState.Maximized;
					Application.Current.MainWindow.Activate();
				}
			}
			if (ClientSettings.AutoActivationSettings.IsPlansAutoActivation)
			{
				foreach (var journalItem in journalItems)
				{
					var journalItemViewModel = new JournalItemViewModel(journalItem);

					var globalStateClass = SKDManager.GetMinStateClass();

					if (journalItem.StateClass <= globalStateClass ||
						(globalStateClass != XStateClass.Fire1 && globalStateClass != XStateClass.Fire2 && globalStateClass != XStateClass.Attention))
					{
						switch (journalItem.JournalItemType)
						{
							case JournalItemType.Device:
								var device = SKDManager.Devices.FirstOrDefault(x => x.UID == journalItem.DeviceUID);
								if (device != null)
								{
									var existsOnPlan = FiresecManager.PlansConfiguration.AllPlans.Any(x => { return x.ElementSKDDevices.Any(y => y.DeviceUID == device.UID); });
									if (existsOnPlan)
									{
										ServiceFactory.Events.GetEvent<ShowSKDDeviceOnPlanEvent>().Publish(device);
									}
								}
								break;

							//case JournalItemType.Zone:
							//	var zone = XManager.Zones.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
							//	if (zone != null)
							//	{
							//		var existsOnPlan = FiresecManager.PlansConfiguration.AllPlans.Any(x => { return x.ElementRectangleXZones.Any(y => y.ZoneUID == zone.UID); });
							//		if (existsOnPlan)
							//		{
							//			ServiceFactory.Events.GetEvent<ShowSKDZoneOnPlanEvent>().Publish(zone);
							//		}
							//		existsOnPlan = FiresecManager.PlansConfiguration.AllPlans.Any(x => { return x.ElementPolygonXZones.Any(y => y.ZoneUID == zone.UID); });
							//		if (existsOnPlan)
							//		{
							//			ServiceFactory.Events.GetEvent<ShowSKDZoneOnPlanEvent>().Publish(zone);
							//		}
							//	}
							//	break;
						}
					}
				}
			}
		}
	}
}