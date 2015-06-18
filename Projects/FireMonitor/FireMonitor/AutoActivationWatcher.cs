﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Infrastructure;
using Infrastructure.Events;
using FiresecAPI.Journal;
using FiresecAPI.GK;
using System.Windows;
using FiresecClient;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace FireMonitor
{
	internal class AutoActivationWatcher
	{
		public void Run()
		{
			ServiceFactory.Events.GetEvent<NewJournalItemsEvent>().Unsubscribe(OnNewJournal);
			ServiceFactory.Events.GetEvent<NewJournalItemsEvent>().Subscribe(OnNewJournal);
		}

		void OnNewJournal(List<JournalItem> journalItems)
		{
			AutoActivate();
			if (ClientSettings.AutoActivationSettings.IsPlansAutoActivation)
				foreach (var journalItem in journalItems)
				{
					var gkStateClass = GKManager.GetMinStateClass();
					var skdStateClass = SKDManager.GetMinStateClass();
					var globalStateClass = (XStateClass)Math.Min((int)gkStateClass, (int)skdStateClass);
					var stateClass = EventDescriptionAttributeHelper.ToStateClass(journalItem.JournalEventNameType);
					if (stateClass <= globalStateClass || (globalStateClass != XStateClass.Fire1 && globalStateClass != XStateClass.Fire2 && globalStateClass != XStateClass.Attention))
						switch (journalItem.JournalObjectType)
						{
							case JournalObjectType.GKDevice:
								var device = GKManager.Devices.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
								if (ShowOnPlanHelper.CanShowDevice(device))
									ShowOnPlanHelper.ShowDevice(device);
								break;
							case JournalObjectType.SKDDevice:
								var skdDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
								if (ShowOnPlanHelper.CanShowSKDDevice(skdDevice))
									ShowOnPlanHelper.ShowSKDDevice(skdDevice);
								break;
							case JournalObjectType.SKDZone:
								var skdZone = SKDManager.Zones.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
								if (ShowOnPlanHelper.CanShowSKDZone(skdZone))
									ShowOnPlanHelper.ShowSKDZone(skdZone);
								break;
							case JournalObjectType.SKDDoor:
								var skdDoor = SKDManager.Doors.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
								if (ShowOnPlanHelper.CanShowSKDDoor(skdDoor))
									ShowOnPlanHelper.ShowSKDDoor(skdDoor);
								break;
							case JournalObjectType.VideoDevice:
								var camera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
								if (ShowOnPlanHelper.CanShowCamera(camera))
									ShowOnPlanHelper.ShowCamera(camera);
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