using System;
using System.IO;
using System.Runtime.Serialization;
using FiresecAPI.Models;
using Infrastructure.Models;

namespace Infrastructure
{
    public static class ClientSettings
    {
        public static readonly string SettingsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ClientSettings");
        public static readonly string ArchiveDefaultStateFileName = Path.Combine(SettingsDirectory, "ArchiveDefaultState.xml");
        public static readonly string AutoActivationSettingsFileName = Path.Combine(SettingsDirectory, "AutoActivationSettings.xml");

        static ArchiveDefaultState _archiveDefaultState;
        public static ArchiveDefaultState ArchiveDefaultState
        {
            get
            {
                if (_archiveDefaultState == null)
                    _archiveDefaultState = new ArchiveDefaultState() { ArchiveDefaultStateType = ArchiveDefaultStateType.All };

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
                    _autoActivationSettings = new AutoActivationSettings() { IsAutoActivation = true, IsPlansAutoActivation = true };

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
            catch { }

            try
            {
                LoadAutoActivationSettings();
            }
            catch { }
        }

        public static void SaveSettings()
        {
            try
            {
                if (Directory.Exists(SettingsDirectory) == false)
                    Directory.CreateDirectory(SettingsDirectory);

                SaveArchiveDefaultState();
                SaveAutoActivationSettings();
            }
            catch { }
        }

        public static void LoadArchiveDefaultState()
        {
            using (var fileStream = new FileStream(ArchiveDefaultStateFileName, FileMode.Open))
            {
                var dataContractSerializer = new DataContractSerializer(typeof(ArchiveDefaultState));
                ArchiveDefaultState = (ArchiveDefaultState) dataContractSerializer.ReadObject(fileStream);
            }
        }

        public static void LoadAutoActivationSettings()
        {
            using (var fileStream = new FileStream(AutoActivationSettingsFileName, FileMode.Open))
            {
                var dataContractSerializer = new DataContractSerializer(typeof(AutoActivationSettings));
                AutoActivationSettings = (AutoActivationSettings) dataContractSerializer.ReadObject(fileStream);
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