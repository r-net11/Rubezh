using System;
using System.IO;
using System.Runtime.Serialization;
using Common;
using Infrastructure.Models;
using Infrastructure.Common;

namespace Infrastructure
{
	public static class ClientSettings
	{
		public static readonly string ArchiveDefaultStateFileName = AppDataFolderHelper.GetMonitorSettingsPath("ArchiveDefaultState.xml");
		public static readonly string AutoActivationSettingsFileName = AppDataFolderHelper.GetMonitorSettingsPath("AutoActivationSettings.xml");

		static ArchiveDefaultState _archiveDefaultState;
		public static ArchiveDefaultState ArchiveDefaultState
		{
			get
			{
				if (_archiveDefaultState == null)
					_archiveDefaultState = new ArchiveDefaultState();

				return _archiveDefaultState;
			}
			set { _archiveDefaultState = value; }
		}

		static AutoActivationSettings _autoActivationSettings;
		public static AutoActivationSettings AutoActivationSettings
		{
			get
			{
				if (_autoActivationSettings == null)
					_autoActivationSettings = new AutoActivationSettings();

				return _autoActivationSettings;
			}
			set { _autoActivationSettings = value; }
		}

		public static void LoadSettings()
		{
			try
			{
				LoadArchiveDefaultState();
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове ClientSettings.LoadSettings");
			}

			try
			{
				LoadAutoActivationSettings();
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове ClientSettings.LoadAutoActivationSettings");
			}
		}

		public static void SaveSettings()
		{
			try
			{
				if (Directory.Exists(AppDataFolderHelper.GetMonitorSettingsPath()) == false)
					Directory.CreateDirectory(AppDataFolderHelper.GetMonitorSettingsPath());

				SaveArchiveDefaultState();
				SaveAutoActivationSettings();
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове ClientSettings.SaveSettings");
			}
		}

		public static void LoadArchiveDefaultState()
		{
			if (File.Exists(ArchiveDefaultStateFileName))
			{
				using (var fileStream = new FileStream(ArchiveDefaultStateFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					var dataContractSerializer = new DataContractSerializer(typeof(ArchiveDefaultState));
					ArchiveDefaultState = (ArchiveDefaultState)dataContractSerializer.ReadObject(fileStream);
				}
			}
			else
			{
				ArchiveDefaultState = new ArchiveDefaultState();
			}
		}

		public static void LoadAutoActivationSettings()
		{
			if (File.Exists(AutoActivationSettingsFileName))
			{
				using (var fileStream = new FileStream(AutoActivationSettingsFileName, FileMode.Open))
				{
					var dataContractSerializer = new DataContractSerializer(typeof(AutoActivationSettings));
					AutoActivationSettings = (AutoActivationSettings)dataContractSerializer.ReadObject(fileStream);
				}
			}
			else
			{
				AutoActivationSettings = new AutoActivationSettings();
			}
		}

		public static void SaveArchiveDefaultState()
		{
			using (var fileStream = new FileStream(ArchiveDefaultStateFileName, FileMode.Create))
			{
				var dataContractSerializer = new DataContractSerializer(typeof(ArchiveDefaultState));
				dataContractSerializer.WriteObject(fileStream, ArchiveDefaultState);
			}
		}

		public static void SaveAutoActivationSettings()
		{
			using (var fileStream = new FileStream(AutoActivationSettingsFileName, FileMode.Create))
			{
				var dataContractSerializer = new DataContractSerializer(typeof(AutoActivationSettings));
				dataContractSerializer.WriteObject(fileStream, AutoActivationSettings);
			}
		}
	}
}