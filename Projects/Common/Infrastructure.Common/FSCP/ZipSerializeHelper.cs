﻿using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Common;
using RubezhAPI;
using Infrastructure.Common.Windows;

namespace Infrastructure.Common
{
	public class ZipSerializeHelper
	{
		public static MemoryStream Serialize<T>(T configuration)
			where T : VersionedConfiguration
		{
			configuration.BeforeSave();
			configuration.Version = new ConfigurationVersion() { MajorVersion = 1, MinorVersion = 1 };
			var memoryStream = new MemoryStream();

			var xmlSerializer = new XmlSerializer(configuration.GetType());
			xmlSerializer.Serialize(memoryStream, configuration);
			return memoryStream;
		}

		public static T DeSerialize<T>(Stream stream)
			 where T : VersionedConfiguration, new()
		{
			T configuration = null;
			var xmlSerializer = new XmlSerializer(typeof(T));
			configuration = (T)xmlSerializer.Deserialize(stream);
			configuration.ValidateVersion();
			configuration.AfterLoad();
			return configuration;
		}

		public static bool Serialize<T>(T configuration, string fileName, bool useXml)
			where T : VersionedConfiguration
		{
			try
			{
				if (useXml)
				{
					var xmlSerializer = new XmlSerializer(configuration.GetType());
					using (var fileStream = new FileStream(fileName, FileMode.Create))
					{
						xmlSerializer.Serialize(fileStream, configuration);
					}
				}
				else
				{
					var dataContractSerializer = new DataContractSerializer(configuration.GetType());
					using (var fileStream = new FileStream(fileName, FileMode.Create))
					{
						dataContractSerializer.WriteObject(fileStream, configuration);
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "ZipSerializeHelper.Serialize");
				return false;
			}
			return true;
		}

		public static T DeSerialize<T>(string fileName, bool useXml)
			 where T : VersionedConfiguration, new()
		{
			try
			{
				using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
				{
					T configuration = null;
					if (useXml)
					{
						var xmlSerializer = new XmlSerializer(typeof(T));
						configuration = (T)xmlSerializer.Deserialize(fileStream);
                    }
					else
					{
						var dataContractSerializer = new DataContractSerializer(typeof(T));
						configuration = (T)dataContractSerializer.ReadObject(fileStream);
					}
					fileStream.Close();
					configuration.ValidateVersion();
					configuration.AfterLoad();
					return configuration;
				}
			}
			catch (Exception e)
			{
                MessageBoxService.Show("Deserialoze error " + e.Message);
				Logger.Error("ZipSerializeHelper.DeSerialize " + fileName + " " + e.Message);
				return new T();
			}
		}
	}
}