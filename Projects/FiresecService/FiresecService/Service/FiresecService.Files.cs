using Common;
using StrazhAPI.Journal;
using Infrastructure.Common;
using Ionic.Zip;
using System.Collections.Generic;
using System.IO;

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

		public Stream GetFile(string fileName)
		{
			return new FileStream(AppDataFolderHelper.GetServerAppDataPath(fileName), FileMode.Open, FileAccess.Read);
		}

		public Stream GetConfig()
		{
			var filePath = AppDataFolderHelper.GetServerAppDataPath("Config.fscp");
			return new FileStream(filePath, FileMode.Open, FileAccess.Read);
		}

		public void SetLocalConfig()
		{
			var configFileName = AppDataFolderHelper.GetServerAppDataPath("Config.fscp");
			var configDirectory = AppDataFolderHelper.GetServerAppDataPath("Config");

			CreateZipConfigFromFiles(configFileName, configDirectory);
			RestartWithNewConfig();
		}

		public void SetConfig(Stream stream)
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

			CreateZipConfigFromFiles(configFileName, configDirectory);
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

		private void RestartWithNewConfig()
		{
			AddJournalMessage(JournalEventNameType.Применение_конфигурации, null);
			ConfigurationCashHelper.Update();
			SKDProcessor.SetNewConfig();
			ScheduleRunner.SetNewConfig();
			ProcedureRunner.SetNewConfig();
		}

		private static void CreateZipConfigFromFiles(string configFileName, string configDirectory)
		{
			if (File.Exists(configFileName))
				File.Delete(configFileName);

			var zipFile = new ZipFile(configFileName);
			zipFile.AddDirectory(configDirectory);
			zipFile.Save(configFileName);
			zipFile.Dispose();
		}
	}
}