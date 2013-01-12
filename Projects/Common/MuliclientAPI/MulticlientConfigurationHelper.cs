using System;
using System.IO;
using System.Runtime.Serialization;
using Common;

namespace MuliclientAPI
{
	public static class MulticlientConfigurationHelper
	{
		public static MulticlientConfiguration LoadConfiguration(string password)
		{
			try
			{
				EncryptHelper.DecryptFile("MulticlientConfiguration.xml", "TempConfiguration.xml", password);
			}
			catch (System.Security.Cryptography.CryptographicException)
			{
				return null;
			}
			try
			{
				var memoryStream = new MemoryStream();
				using (var fileStream = new FileStream("TempConfiguration.xml", FileMode.Open))
				{
					memoryStream.SetLength(fileStream.Length);
					fileStream.Read(memoryStream.GetBuffer(), 0, (int)fileStream.Length);
				}
				File.Delete("TempConfiguration.xml");
				var dataContractSerializer = new DataContractSerializer(typeof(MulticlientConfiguration));
				var configuration = (MulticlientConfiguration)dataContractSerializer.ReadObject(memoryStream);
				return configuration;
			}
			catch (Exception e)
			{
				Logger.Error(e, "MulticlientConfigurationHelper.LoadData");
			}
			return null;
		}

		public static void SaveConfiguration(MulticlientConfiguration configuration, string password)
		{
			try
			{
				using (var memoryStream = new MemoryStream())
				{
					var dataContractSerializer = new DataContractSerializer(typeof(MulticlientConfiguration));
					dataContractSerializer.WriteObject(memoryStream, configuration);

					using (var fileStream = new FileStream("TempConfiguration.xml", FileMode.Create))
					{
						fileStream.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Position);
					}
					EncryptHelper.EncryptFile("TempConfiguration.xml", "MulticlientConfiguration.xml", password);
					File.Delete("TempConfiguration.xml");
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "MulticlientConfigurationHelper.SaveData");
			}
		}
	}
}