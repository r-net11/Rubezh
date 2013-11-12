using System;
using System.Collections.Generic;
using GKProcessor;
using FiresecClient;
using GKModule.Events;
using Infrastructure;
using XFiresecAPI;
using GKProcessor.Events;

namespace GKModule
{
	public static class JournaActionlHelper
	{
		public static void Add(string message, string description)
		{
			Add(message, description, XStateClass.Norm, Guid.Empty, 0, JournalItemType.System, null);
		}

		public static void Add(string message, string description, XStateClass stateClass)
		{
			Add(message, description, stateClass, Guid.Empty, 0, JournalItemType.System, null);
		}

		public static void Add(string message, string description, XStateClass stateClass, XDevice device)
		{
			Add(message, description, stateClass, device.UID, device.GKDescriptorNo, JournalItemType.Device, device.DottedPresentationAddress + device.ShortName);
		}

		public static void Add(string message, string description, XStateClass stateClass, XZone zone)
		{
			Add(message, description, stateClass, zone.UID, zone.GKDescriptorNo, JournalItemType.Zone, zone.PresentationName);
		}

		public static void Add(string message, string description, XStateClass stateClass, XDirection direction)
		{
			Add(message, description, stateClass, direction.UID, direction.GKDescriptorNo, JournalItemType.Direction, direction.PresentationName);
		}

		public static void Add(string message, string description, XStateClass stateClass, XDelay delay)
		{
			Add(message, description, stateClass, delay.UID, delay.GKDescriptorNo, JournalItemType.Delay, delay.Name);
		}

		public static void Add(string message, string description, XStateClass stateClass, Guid objectUID, int gkObjectNo, JournalItemType journalItemType, string objectName)
		{
			var journalItem = new JournalItem()
			{
				SystemDateTime = DateTime.Now,
				DeviceDateTime = DateTime.Now,
				JournalItemType = journalItemType,
				StateClass = stateClass,
				Name = message,
				Description = description,
				ObjectUID = objectUID,
				ObjectName = objectName,
				ObjectStateClass = XStateClass.Norm,
				GKObjectNo = (ushort)gkObjectNo,
				UserName = FiresecManager.CurrentUser.Name,
				SubsystemType = XSubsystemType.System
			};
			Add(journalItem);
		}

		public static void Add(JournalItem journalItem)
		{
			GKDBHelper.Add(journalItem);
			ServiceFactory.Events.GetEvent<NewXJournalEvent>().Publish(new List<JournalItem>() { journalItem });
		}
	}
}