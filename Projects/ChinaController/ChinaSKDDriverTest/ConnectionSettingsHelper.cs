using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Windows;
using System.IO;

namespace ControllerSDK
{
	public static class ConnectionSettingsHelper
	{
		static string FileName = "ConnectionSettings.xml";

		public static ConnectionSettings Get()
		{
			try
			{
				var registryDataConfiguration = new ConnectionSettings();
				if (File.Exists(FileName))
				{
					using (var fileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
					{
						var dataContractSerializer = new DataContractSerializer(typeof(ConnectionSettings));
						registryDataConfiguration = (ConnectionSettings)dataContractSerializer.ReadObject(fileStream);
					}
				}
				return registryDataConfiguration;
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
			return new ConnectionSettings();
		}

		public static void Set(ConnectionSettings connectionSettings)
		{
			try
			{
				var dataContractSerializer = new DataContractSerializer(typeof(ConnectionSettings));
				using (var fileStream = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
				{
					dataContractSerializer.WriteObject(fileStream, connectionSettings);
				}
				return;
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}
	}

	[DataContract]
	public class ConnectionSettings
	{
		public ConnectionSettings()
		{
			Address = "172.16.6.55";
			Port = 37777;
			Login = "admin";
			Password = "123456";
		}

		[DataMember]
		public string Address { get; set; }

		[DataMember]
		public int Port { get; set; }

		[DataMember]
		public string Login { get; set; }

		[DataMember]
		public string Password { get; set; }
	}
}