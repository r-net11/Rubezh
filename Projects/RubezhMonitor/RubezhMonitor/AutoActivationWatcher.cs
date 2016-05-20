using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhClient;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace RubezhMonitor
{
	internal class AutoActivationWatcher
	{
		public void Run()
		{
			ServiceFactory.Events.GetEvent<NewJournalItemsEvent>().Unsubscribe(OnNewJournals);
			ServiceFactory.Events.GetEvent<NewJournalItemsEvent>().Subscribe(OnNewJournals);
		}

		void OnNewJournals(List<JournalItem> journalItems)
		{
			AutoActivate();
			if (ClientSettings.AutoActivationSettings.IsPlansAutoActivation)
				foreach (var journalItem in journalItems)
				{
					var globalStateClass = GKManager.GetMinStateClass();
					var stateClass = EventDescriptionAttributeHelper.ToStateClass(journalItem.JournalEventNameType);
					if (stateClass <= globalStateClass || (globalStateClass != XStateClass.Fire1 && globalStateClass != XStateClass.Fire2 && globalStateClass != XStateClass.Attention))
						switch (journalItem.JournalObjectType)
						{
							case JournalObjectType.GKDevice:
								var device = GKManager.Devices.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
								if (device != null)
									ShowOnPlanHelper.ShowObjectOnPlan(device);
								break;
							case JournalObjectType.GKPumpStation:
								var pumpStation = GKManager.PumpStations.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
								if (pumpStation != null)
									ShowOnPlanHelper.ShowObjectOnPlan(pumpStation);
								break;
							case JournalObjectType.GKZone:
								var zone = GKManager.Zones.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
								if (zone != null)
									ShowOnPlanHelper.ShowObjectOnPlan(zone);
								break;
							case JournalObjectType.GKDelay:
								var delay = GKManager.Delays.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
								if (delay!= null)
									ShowOnPlanHelper.ShowObjectOnPlan(delay);
								break;
							case JournalObjectType.GKDirection:
								var direction = GKManager.Directions.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
								if (direction != null)
									ShowOnPlanHelper.ShowObjectOnPlan(direction);
								break;
							case JournalObjectType.GKGuardZone:
								var guardZone = GKManager.GuardZones.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
								if (guardZone!= null)
									ShowOnPlanHelper.ShowObjectOnPlan(guardZone);
								break;
							case JournalObjectType.GKMPT:
								var mpt = GKManager.MPTs.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
								if (mpt != null)
									ShowOnPlanHelper.ShowObjectOnPlan(mpt);
								break;
							case JournalObjectType.GKDoor:
								var door = GKManager.Doors.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
								if (door != null)
									ShowOnPlanHelper.ShowObjectOnPlan(door);
								break;
							case JournalObjectType.Camera:
								var camera = ClientManager.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
								if (camera != null)
									ShowOnPlanHelper.ShowObjectOnPlan(camera);
								break;
						}
				}
		}

		void AutoActivate()
		{
			if (ClientSettings.AutoActivationSettings.IsAutoActivation)
			{
				var window = DialogService.GetActiveWindow();
				if (window != null && !window.IsActive)
				{
					if (ApplicationService.ApplicationWindow != null)
						ApplicationService.ApplicationWindow.WindowState = WindowState.Maximized;
					window.Activate();
				}
			}
		}
	}
}