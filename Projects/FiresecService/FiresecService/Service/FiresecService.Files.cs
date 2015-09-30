using System.Collections.Generic;
using System.IO;
using Common;
using FiresecAPI.Journal;
using Infrastructure.Common;
using Ionic.Zip;
using System.Threading;
using System;
using Infrastructure.Automation;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		public List<string> GetFileNamesList(string directory)
		{
			return HashHelper.GetFileNamesList(AppDataFolderHelper.GetServerAppDataPath(directory));
		}

		public Dictionary<string, string> GetDirectoryHash(string directory)
		{
			return HashHelper.GetDirectoryHash(AppDataFolderHelper.GetServerAppDataPath(directory));
		}

		public Stream GetServerAppDataFile(string dirAndFileName)
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

		public Stream GetServerFile(string filePath)
		{
			try
			{
				if (File.Exists(filePath))
					return new FileStream(filePath, FileMode.Open, FileAccess.Read);
			}
			catch { }
			return Stream.Null;
		}

		public Stream GetConfig()
		{
			var configFilePath = AppDataFolderHelper.GetServerAppDataPath("Config.fscp");
			if (!File.Exists(configFilePath))
			{
				CreateZipConfigFromFiles();
			}
			return new FileStream(configFilePath, FileMode.Open, FileAccess.Read);
		}

		public void SetLocalConfig()
		{
			var configFileName = AppDataFolderHelper.GetServerAppDataPath("Config.fscp");
			CreateZipConfigFromFiles();
			RestartWithNewConfig();
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
			AddJournalMessage(JournalEventNameType.Применение_конфигурации, null);
			ServerState = FiresecAPI.ServerState.Restarting;
			ConfigurationCashHelper.Update();
			GKProcessor.SetNewConfig();
			ScheduleRunner.SetNewConfig();
			AutomationProcessor.SetNewConfig();
			ServerTaskRunner.SetNewConfig();
			ServerState = FiresecAPI.ServerState.Ready;
			NotifyConfigurationChanged();
		}

		static void CreateZipConfigFromFiles()
		{
			var configFileName = AppDataFolderHelper.GetServerAppDataPath("Config.fscp");
			var configDirectory = AppDataFolderHelper.GetServerAppDataPath("Config");

			if (File.Exists(configFileName))
				File.Delete(configFileName);

			var zipFile = new ZipFile(configFileName);
			zipFile.AddDirectory(configDirectory);
			zipFile.Save(configFileName);
			zipFile.Dispose();
		}
	}
}