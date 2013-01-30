using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using Common;

namespace Infrastructure.Common
{
	public class RegistrySettingsHelper
	{
		static string FileName = AppDataFolderHelper.GetRegistryDataConfigurationFileName();

		public static string Get(string name)
		{
			try
			{
				var registryDataConfiguration = GetRegistryDataConfiguration();
				var registryData = registryDataConfiguration.RegistryDataCollection.FirstOrDefault(x => x.Name == name);
				if (registryData != null)
				{
					return registryData.Value;
				}
				return null;
			}
			catch (Exception e)
			{
				Logger.Error(e, "RegistrySettingsHelper.Get");
				return null;
			}
		}

		public static void Set(string name, string value)
		{
			try
			{
				var registryDataConfiguration = GetRegistryDataConfiguration();
				var registryData = registryDataConfiguration.RegistryDataCollection.FirstOrDefault(x => x.Name == name);
				if (registryData == null)
				{
					registryData = new RegistryData()
					{
						Name = name
					};
					registryDataConfiguration.RegistryDataCollection.Add(registryData);
				}
				registryData.Value = value;
				SetRegistryDataConfiguration(registryDataConfiguration);
			}
			catch (Exception e)
			{
				Logger.Error(e, "RegistrySettingsHelper.Set");
			}
		}

		static RegistryDataConfiguration GetRegistryDataConfiguration()
		{
			var registryDataConfiguration = new RegistryDataConfiguration();
			if (File.Exists(FileName))
			{
				using (var fileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					var dataContractSerializer = new DataContractSerializer(typeof(RegistryDataConfiguration));
					registryDataConfiguration = (RegistryDataConfiguration)dataContractSerializer.ReadObject(fileStream);
				}
			}
			return registryDataConfiguration;
		}

		static void SetRegistryDataConfiguration(RegistryDataConfiguration registryDataConfiguration)
		{
			var dataContractSerializer = new DataContractSerializer(typeof(RegistryDataConfiguration));
			using (var fileStream = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
			{
				dataContractSerializer.WriteObject(fileStream, registryDataConfiguration);
			}
		}
	}

	[DataContract]
	public class RegistryDataConfiguration
	{
		public RegistryDataConfiguration()
		{
			RegistryDataCollection = new List<RegistryData>();
		}

		[DataMember]
		public List<RegistryData> RegistryDataCollection { get; set; }
	}

	[DataContract]
	public class RegistryData
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Value { get; set; }
	}
}