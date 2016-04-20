using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Common;
using FiresecAPI.Models;
using Infrastructure.Common.Windows;
using System.Diagnostics;

namespace ServerFS2.Helpers
{
	public class LastJournalIndexManager
	{
		readonly string FileName = Path.Combine(AppDataFolderHelper.GetFile("DeviceLastIndexCollection.xml"));
		object Locker = new object();
		public DeviceLastIndexCollection DeviceLastIndexCollection;

		public LastJournalIndexManager()
		{
			Load();
		}

		public void SetLastFireJournalIndex(Device device, string serialNo, int value)
		{
			var deviceLastJournalIndex = DeviceLastIndexCollection.DeviceLastJournalIndexes.FirstOrDefault(x => x.DeviceUID == device.UID && x.SerialNo == serialNo);
			if (deviceLastJournalIndex == null)
			{
				deviceLastJournalIndex = new DeviceLastJournalIndex()
				{
					DeviceUID = device.UID,
					SerialNo = serialNo
				};
				DeviceLastIndexCollection.DeviceLastJournalIndexes.Add(deviceLastJournalIndex);
			}
			deviceLastJournalIndex.LastFireJournalIndex = value;
			Save();
		}

		public int GetLastFireJournalIndex(Device device, string serialNo)
		{
			var deviceLastJournalIndex = DeviceLastIndexCollection.DeviceLastJournalIndexes.FirstOrDefault(x => x.DeviceUID == device.UID && x.SerialNo == serialNo);
			if (deviceLastJournalIndex != null)
			{
				return deviceLastJournalIndex.LastFireJournalIndex;
			}
			return 0;
		}

		public void SetLastSecurityJournalIndex(Device device, string serialNo, int value)
		{
			var deviceLastJournalIndex = DeviceLastIndexCollection.DeviceLastJournalIndexes.FirstOrDefault(x => x.DeviceUID == device.UID && x.SerialNo == serialNo);
			if (deviceLastJournalIndex == null)
			{
				deviceLastJournalIndex = new DeviceLastJournalIndex()
				{
					DeviceUID = device.UID,
					SerialNo = serialNo
				};
				DeviceLastIndexCollection.DeviceLastJournalIndexes.Add(deviceLastJournalIndex);
			}
			deviceLastJournalIndex.LastSecurityJournalIndex = value;
			Save();
		}

		public int GetLastSecurityJournalIndex(Device device, string serialNo)
		{
			var deviceLastJournalIndex = DeviceLastIndexCollection.DeviceLastJournalIndexes.FirstOrDefault(x => x.DeviceUID == device.UID && x.SerialNo == serialNo);
			if (deviceLastJournalIndex != null)
			{
				return deviceLastJournalIndex.LastSecurityJournalIndex;
			}
			return 0;
		}

		void Load()
		{
			lock (Locker)
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
					Logger.Error(e, "LastJournalIndexManager.Load");
				}
			}
		}

		void Save()
		{
			lock (Locker)
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
					Logger.Error(e, "LastJournalIndexManager.Save");
				}
			}
		}
	}
}