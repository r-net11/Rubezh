using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FiresecAPI.GK;
using FiresecClient;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Events;

namespace GKModule
{
	public static class AutoActivationWatcher
	{
		public static void Run()
		{
			ServiceFactory.Events.GetEvent<NewXJournalEvent>().Unsubscribe(OnNewJournal);
			ServiceFactory.Events.GetEvent<NewXJournalEvent>().Subscribe(OnNewJournal);
		}

		public static void OnNewJournal(List<GKJournalItem> journalItems)
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

					var globalStateClass = GKManager.GetMinStateClass();

					if (journalItem.StateClass <= globalStateClass ||
						(globalStateClass != XStateClass.Fire1 && globalStateClass != XStateClass.Fire2 && globalStateClass != XStateClass.Attention))
					{
						switch (journalItem.JournalObjectType)
						{
							case GKJournalObjectType.Device:
								var device = GKManager.Devices.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
								if (device != null)
								{
									var existsOnPlan = FiresecManager.PlansConfiguration.AllPlans.Any(x => { return x.ElementGKDevices.Any(y => y.DeviceUID == device.UID); });
									if (existsOnPlan)
									{
										ServiceFactory.Events.GetEvent<ShowXDeviceOnPlanEvent>().Publish(device);
									}
								}
								break;

							case GKJournalObjectType.Zone:
								var zone = GKManager.Zones.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
								if (zone != null)
								{
									var existsOnPlan = FiresecManager.PlansConfiguration.AllPlans.Any(x => { return x.ElementRectangleGKZones.Any(y => y.ZoneUID == zone.UID); });
									if (existsOnPlan)
									{
										ServiceFactory.Events.GetEvent<ShowXZoneOnPlanEvent>().Publish(zone);
									}
									existsOnPlan = FiresecManager.PlansConfiguration.AllPlans.Any(x => { return x.ElementPolygonGKZones.Any(y => y.ZoneUID == zone.UID); });
									if (existsOnPlan)
									{
										ServiceFactory.Events.GetEvent<ShowXZoneOnPlanEvent>().Publish(zone);
									}
								}
								break;

							case GKJournalObjectType.Direction:
								var direction = GKManager.Directions.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
								if (direction != null)
								{
									var existsOnPlan = FiresecManager.PlansConfiguration.AllPlans.Any(x => { return x.ElementRectangleGKDirections.Any(y => y.DirectionUID == direction.UID); });
									if (existsOnPlan)
									{
										ServiceFactory.Events.GetEvent<ShowXDirectionOnPlanEvent>().Publish(direction);
									}
									existsOnPlan = FiresecManager.PlansConfiguration.AllPlans.Any(x => { return x.ElementPolygonGKDirections.Any(y => y.DirectionUID == direction.UID); });
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