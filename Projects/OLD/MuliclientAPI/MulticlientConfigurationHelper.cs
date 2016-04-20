using System;
using System.IO;
using System.Runtime.Serialization;
using Common;
using Infrastructure.Common.Windows;

namespace MuliclientAPI
{
	public static class MulticlientConfigurationHelper
	{
		public static MulticlientConfiguration LoadConfiguration(string password)
		{
			try
			{
				EncryptHelper.DecryptFile(AppDataFolderHelper.GetMulticlientFile(), AppDataFolderHelper.GetTempMulticlientFile(), password);
			}
			catch (System.Security.Cryptography.CryptographicException)
			{
				return null;
			}
			try
			{
				var memoryStream = new MemoryStream();
				using (var fileStream = new FileStream(AppDataFolderHelper.GetTempMulticlientFile(), FileMode.Open))
				{
					memoryStream.SetLength(fileStream.Length);
					fileStream.Read(memoryStream.GetBuffer(), 0, (int)fileStream.Length);
				}
				File.Delete(AppDataFolderHelper.GetTempMulticlientFile());
				var dataContractSerializer = new DataContractSerializer(typeof(MulticlientConfiguration));
				var configuration = (MulticlientConfiguration)dataContractSerializer.ReadObject(memoryStream);
				return configuration;
			}
			catch (Exception e)
			{
				Logger.Error(e, "MulticlientConfigurationHelper.LoadConfiguration");
			}
			return null;
		}

		public static void SaveConfiguration(MulticlientConfiguration configuration, string password)
		{
			try
			{
				if (!Directory.Exists(AppDataFolderHelper.GetMulticlientDirectory()))
					Directory.CreateDirectory(AppDataFolderHelper.GetMulticlientDirectory());

				using (var memoryStream = new MemoryStream())
				{
					var dataContractSerializer = new DataContractSerializer(typeof(MulticlientConfiguration));
					dataContractSerializer.WriteObject(memoryStream, configuration);

					using (var fileStream = new FileStream(AppDataFolderHelper.GetTempMulticlientFile(), FileMode.Create))
					{
						fileStream.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Position);
					}
					EncryptHelper.EncryptFile(AppDataFolderHelper.GetTempMulticlientFile(), AppDataFolderHelper.GetMulticlientFile(), password);
					File.Delete(AppDataFolderHelper.GetTempMulticlientFile());
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "MulticlientConfigurationHelper.SaveConfiguration");
			}
		}
	}
}