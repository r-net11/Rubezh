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

		public static void Add(string message, string description, XStateClass stateClass, XBase xBase)
		{
			Guid uid = Guid.Empty;
			JournalItemType journalItemType = JournalItemType.System;
			if (xBase is XDevice)
			{
				uid = (xBase as XDevice).UID;
				journalItemType = JournalItemType.Device;
			}
			if (xBase is XZone)
			{
				uid = (xBase as XZone).UID;
				journalItemType = JournalItemType.Zone;
			}
			if (xBase is XDirection)
			{
				uid = (xBase as XDirection).UID;
				journalItemType = JournalItemType.Direction;
			}
			if (xBase is XDelay)
			{
				uid = (xBase as XDelay).UID;
				journalItemType = JournalItemType.Delay;
			}
			Add(message, description, stateClass, uid, xBase.GKDescriptorNo, journalItemType, xBase.PresentationName);
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