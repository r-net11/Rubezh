using System;
using System.IO;
using System.Runtime.Serialization;
using Common;
using Infrastructure.Common;
using System.Collections.Generic;

namespace GKImitator.Processor
{
	public static class DBHelper
	{
		static string FileName = AppDataFolderHelper.GetServerAppDataPath("ImitatorSerializedCollection.xml");
		public static ImitatorSerializedCollection ImitatorSerializedCollection { get; set; }

		static DBHelper()
		{
			Load();
			if (ImitatorSerializedCollection.ImitatorSchedules == null)
				ImitatorSerializedCollection.ImitatorSchedules = new List<ImitatorSchedule>();
		}

		public static void Load()
		{
			try
			{
				ImitatorSerializedCollection = new ImitatorSerializedCollection();
				if (File.Exists(FileName))
				{
					using (var fileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
					{
						var dataContractSerializer = new DataContractSerializer(typeof(ImitatorSerializedCollection));
						ImitatorSerializedCollection = (ImitatorSerializedCollection)dataContractSerializer.ReadObject(fileStream);
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "DBHelper.Load");
			}
		}

		public static void Save()
		{
			try
			{
				var dataContractSerializer = new DataContractSerializer(typeof(ImitatorSerializedCollection));
				using (var fileStream = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
				{
					dataContractSerializer.WriteObject(fileStream, ImitatorSerializedCollection);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "DBHelper.Save");
			}
		}
	}
}