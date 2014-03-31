using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Common;
using Infrastructure.Common;
using Infrastructure.Models;

namespace Infrastructure
{
	public static class ClientSettings
	{
		public static readonly string ArchiveDefaultStateFileName = AppDataFolderHelper.GetMonitorSettingsPath("ArchiveDefaultState.xml");
		public static readonly string AutoActivationSettingsFileName = AppDataFolderHelper.GetMonitorSettingsPath("AutoActivationSettings.xml");
		public static readonly string MultiLayoutCameraSettingsFileName = AppDataFolderHelper.GetMonitorSettingsPath("MultiLayoutCameraSettings.xml");
		public static readonly string RviMultiLayoutCameraSettingsFileName = AppDataFolderHelper.GetMonitorSettingsPath("RviMultiLayoutCameraSettings.xml");
		public static readonly string SKDSettingsFileName = AppDataFolderHelper.GetMonitorSettingsPath("SKDSettings.xml");

		static ArchiveDefaultState _archiveDefaultState;
		public static ArchiveDefaultState ArchiveDefaultState
		{
			get { return _archiveDefaultState ?? (_archiveDefaultState = new ArchiveDefaultState()); }
			set { _archiveDefaultState = value; }
		}

		static AutoActivationSettings _autoActivationSettings;
		public static AutoActivationSettings AutoActivationSettings
		{
			get { return _autoActivationSettings ?? (_autoActivationSettings = new AutoActivationSettings()); }
			set { _autoActivationSettings = value; }
		}

		static MultiLayoutCameraSettings _multiLayoutCameraSettings;
		public static MultiLayoutCameraSettings MultiLayoutCameraSettings
		{
			get { return _multiLayoutCameraSettings ?? (_multiLayoutCameraSettings = new MultiLayoutCameraSettings()); }
			set { _multiLayoutCameraSettings = value; }
		}

		static RviMultiLayoutCameraSettings _rviMultiLayoutCameraSettings;
		public static RviMultiLayoutCameraSettings RviMultiLayoutCameraSettings
		{
			get { return _rviMultiLayoutCameraSettings ?? (_rviMultiLayoutCameraSettings = new RviMultiLayoutCameraSettings()); }
			set { _rviMultiLayoutCameraSettings = value; }
		}

		static SKDSettings _skdSettings;
		public static SKDSettings SKDSettings
		{
			get { return _skdSettings ?? (_skdSettings = new SKDSettings()); }
			set { _skdSettings = value; }
		}

		public static void LoadSettings()
		{
			try
			{
				LoadArchiveDefaultState();
				LoadAutoActivationSettings();
				LoadCameraSettings();
				LoadRviCameraSettings();
				LoadSKDSettings();
			}
			catch (Exception e)
			{
				Logger.Error(e, "ClientSettings.LoadSettings");
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
				SaveCameraSettings();
				SaveRviCameraSettings();
				SaveSKDSettings();
			}
			catch (Exception e)
			{
				Logger.Error(e, "ClientSettings.SaveSettings");
			}
		}

		static void LoadArchiveDefaultState()
		{
			if (File.Exists(ArchiveDefaultStateFileName))
			{
				using (var fileStream = new FileStream(ArchiveDefaultStateFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					var dataContractSerializer = new DataContractSerializer(typeof(ArchiveDefaultState));
					ArchiveDefaultState = (ArchiveDefaultState)dataContractSerializer.ReadObject(fileStream);
					if (ArchiveDefaultState.AdditionalColumns == null)
						ArchiveDefaultState.AdditionalColumns = new List<JournalColumnType>();
				}
			}
			else
			{
				ArchiveDefaultState = new ArchiveDefaultState();
			}
		}

		static void LoadAutoActivationSettings()
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

		static void LoadCameraSettings()
		{
			if (File.Exists(MultiLayoutCameraSettingsFileName))
			{
				using (var fileStream = new FileStream(MultiLayoutCameraSettingsFileName, FileMode.Open))
				{
					var dataContractSerializer = new DataContractSerializer(typeof(MultiLayoutCameraSettings));
					MultiLayoutCameraSettings = (MultiLayoutCameraSettings)dataContractSerializer.ReadObject(fileStream);
					if (MultiLayoutCameraSettings.Dictionary == null)
						MultiLayoutCameraSettings.Dictionary = new Dictionary<string, Guid>();
				}
			}
			else
			{
				MultiLayoutCameraSettings = new MultiLayoutCameraSettings();
			}
		}

		static void LoadRviCameraSettings()
		{
			if (File.Exists(RviMultiLayoutCameraSettingsFileName))
			{
				using (var fileStream = new FileStream(RviMultiLayoutCameraSettingsFileName, FileMode.Open))
				{
					var dataContractSerializer = new DataContractSerializer(typeof(RviMultiLayoutCameraSettings));
					RviMultiLayoutCameraSettings = (RviMultiLayoutCameraSettings)dataContractSerializer.ReadObject(fileStream);
					if (RviMultiLayoutCameraSettings.Dictionary == null)
						RviMultiLayoutCameraSettings.Dictionary = new Dictionary<string, Guid>();
				}
			}
			else
			{
				MultiLayoutCameraSettings = new MultiLayoutCameraSettings();
			}
		}

		static void LoadSKDSettings()
		{
			if (File.Exists(SKDSettingsFileName))
			{
				using (var fileStream = new FileStream(SKDSettingsFileName, FileMode.Open))
				{
					var dataContractSerializer = new DataContractSerializer(typeof(SKDSettings));
					SKDSettings = (SKDSettings)dataContractSerializer.ReadObject(fileStream);
				}
			}
			else
			{
				SKDSettings = new SKDSettings();
			}
		}

		static void SaveArchiveDefaultState()
		{
			using (var fileStream = new FileStream(ArchiveDefaultStateFileName, FileMode.Create))
			{
				var dataContractSerializer = new DataContractSerializer(typeof(ArchiveDefaultState));
				dataContractSerializer.WriteObject(fileStream, ArchiveDefaultState);
			}
		}

		static void SaveAutoActivationSettings()
		{
			using (var fileStream = new FileStream(AutoActivationSettingsFileName, FileMode.Create))
			{
				var dataContractSerializer = new DataContractSerializer(typeof(AutoActivationSettings));
				dataContractSerializer.WriteObject(fileStream, AutoActivationSettings);
			}
		}

		static void SaveCameraSettings()
		{
			using (var fileStream = new FileStream(MultiLayoutCameraSettingsFileName, FileMode.Create))
			{
				var dataContractSerializer = new DataContractSerializer(typeof(MultiLayoutCameraSettings));
				dataContractSerializer.WriteObject(fileStream, MultiLayoutCameraSettings);
			}
		}

		static void SaveRviCameraSettings()
		{
			using (var fileStream = new FileStream(RviMultiLayoutCameraSettingsFileName, FileMode.Create))
			{
				var dataContractSerializer = new DataContractSerializer(typeof(RviMultiLayoutCameraSettings));
				dataContractSerializer.WriteObject(fileStream, RviMultiLayoutCameraSettings);
			}
		}

		static void SaveSKDSettings()
		{
			using (var fileStream = new FileStream(SKDSettingsFileName, FileMode.Create))
			{
				var dataContractSerializer = new DataContractSerializer(typeof(SKDSettings));
				dataContractSerializer.WriteObject(fileStream, SKDSettings);
			}
		}
	}
}