using System;
using System.Collections.Generic;
using Common.GK;
using FiresecClient;
using GKModule.Events;
using Infrastructure;
using XFiresecAPI;

namespace GKModule
{
	public static class JournaActionlHelper
	{
		public static void Add(string message, string description)
		{
			Add(message, description, XStateClass.Info, Guid.Empty, 0, JournalItemType.System);
		}

		public static void Add(string message, string description, XStateClass stateClass)
		{
			Add(message, description, stateClass, Guid.Empty, 0, JournalItemType.System);
		}

		public static void Add(string message, string description, XStateClass stateClass, XDevice device)
		{
			Add(message, description, stateClass, device.UID, device.GetDatabaseNo(DatabaseType.Gk), JournalItemType.Device);
		}

		public static void Add(string message, string description, XStateClass stateClass, XZone zone)
		{
			Add(message, description, stateClass, zone.UID, zone.GetDatabaseNo(DatabaseType.Gk), JournalItemType.Zone);
		}

		public static void Add(string message, string description, XStateClass stateClass, XDirection direction)
		{
			Add(message, description, stateClass, direction.UID, direction.GetDatabaseNo(DatabaseType.Gk), JournalItemType.Direction);
		}

		public static void Add(string message, string description, XStateClass stateClass, Guid objectUID, int gkObjectNo, JournalItemType journalItemType)
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
				GKObjectNo = (ushort)gkObjectNo,
				UserName = FiresecManager.CurrentUser.Name
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