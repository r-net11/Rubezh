using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS2Api;
using ServerFS2.Service;
using FiresecAPI;
using FiresecAPI.Models;

namespace ServerFS2.Journal
{
	public static class CustomMessageJournalHelper
	{
		static object Locker = new object();

		public static void Add(string description, string userName, Device panelDevice = null, Device device = null, Zone zone = null)
		{
			var journalItem = new FS2JournalItem
			{
				DeviceTime = DateTime.Now,
				SystemTime = DateTime.Now,
				Description = description,
				UserName = userName,
				PanelDevice = panelDevice,
				Device = device,
				StateType = StateType.Info,
				SubsystemType = SubsystemType.Other,
			};
			if (panelDevice != null)
			{
				journalItem.PanelName = panelDevice.DottedPresentationNameAndAddress;
				journalItem.PanelUID = panelDevice.UID;
			}
			if (device != null)
			{
				journalItem.DeviceName = device.DottedPresentationNameAndAddress;
				journalItem.DeviceUID = device.UID;
			}
			if (zone != null)
			{
				journalItem.ZoneNo = zone.No;
				journalItem.ZoneName = zone.PresentationName;
			}

			AddJournalItem(journalItem);
		}

		static void AddJournalItem(FS2JournalItem fsJournalItem)
		{
			lock (Locker)
			{
				CallbackManager.NewJournalItems(new List<FS2JournalItem>() { fsJournalItem });
				DatabaseHelper.AddJournalItem(fsJournalItem);
			}
		}
	}
}