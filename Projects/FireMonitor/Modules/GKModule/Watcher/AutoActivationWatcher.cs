using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FiresecClient;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Events;
using XFiresecAPI;

namespace GKModule
{
	public static class AutoActivationWatcher
	{
		public static void Run()
		{
			ServiceFactory.Events.GetEvent<NewXJournalEvent>().Unsubscribe(OnNewJournal);
			ServiceFactory.Events.GetEvent<NewXJournalEvent>().Subscribe(OnNewJournal);
		}

		public static void OnNewJournal(List<JournalItem> journalItems)
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

					var globalStateClass = XManager.GetMinStateClass();

					if (journalItem.StateClass <= globalStateClass ||
						(globalStateClass != XStateClass.Fire1 && globalStateClass != XStateClass.Fire2 && globalStateClass != XStateClass.Attention))
					{
						switch (journalItem.JournalItemType)
						{
							case JournalItemType.Device:
								var device = XManager.Devices.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
								if (device != null)
								{
									var existsOnPlan = FiresecManager.PlansConfiguration.AllPlans.Any(x => { return x.ElementXDevices.Any(y => y.XDeviceUID == device.UID); });
									if (existsOnPlan)
									{
										ServiceFactory.Events.GetEvent<ShowXDeviceOnPlanEvent>().Publish(device);
									}
								}
								break;

							case JournalItemType.Zone:
								var zone = XManager.Zones.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
								if (zone != null)
								{
									var existsOnPlan = FiresecManager.PlansConfiguration.AllPlans.Any(x => { return x.ElementRectangleXZones.Any(y => y.ZoneUID == zone.UID); });
									if (existsOnPlan)
									{
										ServiceFactory.Events.GetEvent<ShowXZoneOnPlanEvent>().Publish(zone);
									}
									existsOnPlan = FiresecManager.PlansConfiguration.AllPlans.Any(x => { return x.ElementPolygonXZones.Any(y => y.ZoneUID == zone.UID); });
									if (existsOnPlan)
									{
										ServiceFactory.Events.GetEvent<ShowXZoneOnPlanEvent>().Publish(zone);
									}
								}
								break;

							case JournalItemType.Direction:
								var direction = XManager.Directions.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
								if (direction != null)
								{
									var existsOnPlan = FiresecManager.PlansConfiguration.AllPlans.Any(x => { return x.ElementRectangleXDirections.Any(y => y.DirectionUID == direction.UID); });
									if (existsOnPlan)
									{
										ServiceFactory.Events.GetEvent<ShowXDirectionOnPlanEvent>().Publish(direction);
									}
									existsOnPlan = FiresecManager.PlansConfiguration.AllPlans.Any(x => { return x.ElementPolygonXDirections.Any(y => y.DirectionUID == direction.UID); });
									if (existsOnPlan)
									{
										ServiceFactory.Events.GetEvent<ShowXDirectionOnPlanEvent>().Publish(direction);
									}
								}
								break;
						}
					}
				}
			}
		}
	}
}