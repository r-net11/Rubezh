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
        public static MultiLayoutCameraSettings MultiLayoutMultiLayoutCameraSettings
        {
            get { return _multiLayoutCameraSettings ?? (_multiLayoutCameraSettings = new MultiLayoutCameraSettings()); }
            set { _multiLayoutCameraSettings = value; }
        }

		public static void LoadSettings()
		{
			try
			{
				LoadArchiveDefaultState();
				LoadAutoActivationSettings();
                LoadCameraSettings();
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
                    MultiLayoutMultiLayoutCameraSettings = (MultiLayoutCameraSettings)dataContractSerializer.ReadObject(fileStream);
                }
            }
            else
            {
                MultiLayoutMultiLayoutCameraSettings = new MultiLayoutCameraSettings();
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
                dataContractSerializer.WriteObject(fileStream, MultiLayoutMultiLayoutCameraSettings);
            }
        }
	}
}