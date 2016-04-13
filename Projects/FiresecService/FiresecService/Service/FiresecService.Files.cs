using Common;
using FiresecService.Processor;
using Infrastructure.Automation;
using Infrastructure.Common;
using Ionic.Zip;
using RubezhAPI;
using RubezhAPI.Journal;
using RubezhAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		public List<string> GetFileNamesList(Guid clientUID, string directory)
		{
			return HashHelper.GetFileNamesList(AppDataFolderHelper.GetServerAppDataPath(directory));
		}

		public Dictionary<string, string> GetDirectoryHash(Guid clientUID, string directory)
		{
			return HashHelper.GetDirectoryHash(AppDataFolderHelper.GetServerAppDataPath(directory));
		}

		public Stream GetServerAppDataFile(Guid clientUID, string dirAndFileName)
		{
			try
			{
				var filePath = AppDataFolderHelper.GetServerAppDataPath(dirAndFileName);
				if (File.Exists(filePath))
					return new FileStream(filePath, FileMode.Open, FileAccess.Read);
			}
			catch { }
			return Stream.Null;
		}

		public Stream GetServerFile(Guid clientUID, string filePath)
		{
			try
			{
				if (File.Exists(filePath))
					return new FileStream(filePath, FileMode.Open, FileAccess.Read);
			}
			catch { }
			return Stream.Null;
		}

		public Stream GetConfig(Guid clientUID)
		{
			var configFilePath = AppDataFolderHelper.GetServerAppDataPath("Config.fscp");
			if (!File.Exists(configFilePath))
			{
				CreateZipConfigFromFiles();
			}
			return new FileStream(configFilePath, FileMode.Open, FileAccess.Read);
		}

		public OperationResult<bool> SetLocalConfig(Guid clientUid)
		{
			try
			{
				var configFileName = AppDataFolderHelper.GetServerAppDataPath("Config.fscp");
				CreateZipConfigFromFiles();
				AddJournalMessage(JournalEventNameType.Применение_конфигурации, null, null, clientUid);
				RestartWithNewConfig();
				return new OperationResult<bool>(true);
			}
			catch (Exception e)
			{
				return OperationResult<bool>.FromError(e.Message);
			}
		}

		public void SetRemoteConfig(Stream stream)
		{
			var newConfigFileName = AppDataFolderHelper.GetServerAppDataPath("NewConfig.fscp");
			var newConfigDirectory = AppDataFolderHelper.GetServerAppDataPath("NewConfig");
			var configFileName = AppDataFolderHelper.GetServerAppDataPath("Config.fscp");
			var configDirectory = AppDataFolderHelper.GetServerAppDataPath("Config");

			using (var configFileStream = File.Create(newConfigFileName))
			{
				CopyStream(stream, configFileStream);
			}
			stream.Close();

			if (Directory.Exists(newConfigDirectory))
				Directory.Delete(newConfigDirectory, true);
			Directory.CreateDirectory(newConfigDirectory);

			var newZipFile = new ZipFile(newConfigFileName);
			newZipFile.ExtractAll(newConfigDirectory);
			newZipFile.Dispose();
			if (File.Exists(newConfigFileName))
				File.Delete(newConfigFileName);

			if (!Directory.Exists(configDirectory))
				Directory.CreateDirectory(configDirectory);

			if (Directory.Exists(Path.Combine(configDirectory, "Content")))
				Directory.Delete(Path.Combine(configDirectory, "Content"), true);
			if (!Directory.Exists(Path.Combine(configDirectory, "Content")))
				Directory.CreateDirectory(Path.Combine(configDirectory, "Content"));

			foreach (var fileName in Directory.GetFiles(newConfigDirectory))
			{
				var file = Path.GetFileName(fileName);
				File.Copy(fileName, Path.Combine(configDirectory, file), true);
			}
			foreach (var fileName in Directory.GetFiles(Path.Combine(newConfigDirectory, "Content")))
			{
				var file = Path.GetFileName(fileName);
				File.Copy(fileName, Path.Combine(Path.Combine(configDirectory, "Content"), file), true);
			}
			Directory.Delete(newConfigDirectory, true);

			CreateZipConfigFromFiles();
			RestartWithNewConfig();
		}

		public void SetSecurityConfiguration(Guid clientUID, SecurityConfiguration securityConfiguration)
		{
			securityConfiguration.Version = new ConfigurationVersion() { MinorVersion = 1, MajorVersion = 1 };
			ZipSerializeHelper.Serialize(securityConfiguration, Path.Combine(AppDataFolderHelper.GetServerAppDataPath(), "SecurityConfiguration.xml"));
		}

		public static void CopyStream(Stream input, Stream output)
		{
			var buffer = new byte[8 * 1024];
			int length;
			while ((length = input.Read(buffer, 0, buffer.Length)) > 0)
			{
				output.Write(buffer, 0, length);
			}
			output.Close();
		}

		void RestartWithNewConfig()
		{
			ServerState = RubezhAPI.ServerState.Restarting;
			ConfigurationCashHelper.Update();
			GKProcessor.SetNewConfig();
			ScheduleRunner.SetNewConfig();
			AutomationProcessor.SetNewConfig();
			RviProcessor.SetNewConfig();
			ServerTaskRunner.SetNewConfig();
			OpcDaServersProcessor.SetNewConfig();
			OpcDaHelper.UpdateTagList(ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.OpcDaTsServers);
			ServerState = RubezhAPI.ServerState.Ready;
			NotifyConfigurationChanged();
		}

		static void CreateZipConfigFromFiles()
		{
			var configFileName = AppDataFolderHelper.GetServerAppDataPath("Config.fscp");
			var configDirectory = AppDataFolderHelper.GetServerAppDataPath("Config");

			if (File.Exists(configFileName))
				File.Delete(configFileName);
			ReplaceSecurityConfiguration();
			var zipFile = new ZipFile(configFileName);
			zipFile.AddDirectory(configDirectory);
			zipFile.Save(configFileName);
			zipFile.Dispose();
		}

		static void ReplaceSecurityConfiguration()
		{
			var configDirectory = AppDataFolderHelper.GetServerAppDataPath("Config");
			if (File.Exists(configDirectory + "\\SecurityConfiguration.xml"))
			{
				if (!File.Exists(AppDataFolderHelper.GetServerAppDataPath("Config\\..\\SecurityConfiguration.xml")))
					File.Copy(configDirectory + "\\SecurityConfiguration.xml", AppDataFolderHelper.GetServerAppDataPath("Config\\..\\SecurityConfiguration.xml"));
				File.Delete(configDirectory + "\\SecurityConfiguration.xml");
			}
		}
	}
}