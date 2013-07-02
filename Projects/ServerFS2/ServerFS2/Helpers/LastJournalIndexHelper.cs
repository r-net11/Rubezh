using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using Infrastructure.Common;
using Common;
using FiresecAPI.Models;

namespace ServerFS2.Helpers
{
	public static class LastJournalIndexHelper
	{
		static readonly string FileName = Path.Combine(AppDataFolderHelper.GetFile("DeviceLastIndexCollection.xml"));
		static object Locker = new object();
		public static DeviceLastIndexCollection DeviceLastIndexCollection;

		static LastJournalIndexHelper()
		{
			Load();
		}

		public static void SetLastFireJournalIndex(Device device, int value)
		{
			var deviceLastJournalIndex = DeviceLastIndexCollection.DeviceLastJournalIndexes.FirstOrDefault(x => x.DeviceUID == device.UID);
			if (deviceLastJournalIndex != null)
			{
				deviceLastJournalIndex.LastFireJournalIndex = value;
				Save();
			}
		}

		public static int GetLastFireJournalIndex(Device device)
		{
			var deviceLastJournalIndex = DeviceLastIndexCollection.DeviceLastJournalIndexes.FirstOrDefault(x => x.DeviceUID == device.UID);
			if (deviceLastJournalIndex != null)
			{
				return deviceLastJournalIndex.LastFireJournalIndex;
			}
			return 0;
		}

		public static void SetLastSecurityJournalIndex(Device device, int value)
		{
			var deviceLastJournalIndex = DeviceLastIndexCollection.DeviceLastJournalIndexes.FirstOrDefault(x => x.DeviceUID == device.UID);
			if (deviceLastJournalIndex != null)
			{
				deviceLastJournalIndex.LastSecurityJournalIndex = value;
				Save();
			}
		}

		public static int GetLastSecurityJournalIndex(Device device)
		{
			var deviceLastJournalIndex = DeviceLastIndexCollection.DeviceLastJournalIndexes.FirstOrDefault(x => x.DeviceUID == device.UID);
			if (deviceLastJournalIndex != null)
			{
				return deviceLastJournalIndex.LastSecurityJournalIndex;
			}
			return 0;
		}

		public static void Load()
		{
			try
			{
				DeviceLastIndexCollection = new DeviceLastIndexCollection();
				if (File.Exists(FileName))
				{
					using (var fileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
					{
						var dataContractSerializer = new DataContractSerializer(typeof(DeviceLastIndexCollection));
						DeviceLastIndexCollection = (DeviceLastIndexCollection)dataContractSerializer.ReadObject(fileStream);
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, StackTraceHelper.GetStackTrace());
			}
		}

		public static void Save()
		{
			try
			{
				var dataContractSerializer = new DataContractSerializer(typeof(DeviceLastIndexCollection));
				using (var fileStream = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
				{
					dataContractSerializer.WriteObject(fileStream, DeviceLastIndexCollection);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, StackTraceHelper.GetStackTrace());
			}
		}
	}
}