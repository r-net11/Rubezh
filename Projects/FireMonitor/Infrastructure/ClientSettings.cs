using System;
using System.IO;
using System.Runtime.Serialization;
using FiresecAPI.Models;

namespace Infrastructure
{
    public static class ClientSettings
    {
        public static readonly string SettingsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ClientSettings");
        public static readonly string ArchiveDefaultStateFileName = Path.Combine(SettingsDirectory, "ArchiveDefaultState.xml");

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

        public static void LoadSettings()
        {
            try
            {
                LoadArchiveDefaultState();
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

        public static void SaveArchiveDefaultState()
        {
            using (var fileStream = new FileStream(ArchiveDefaultStateFileName, FileMode.Create))
            {
                var dataContractSerializer = new DataContractSerializer(typeof(ArchiveDefaultState));
                dataContractSerializer.WriteObject(fileStream, ArchiveDefaultState);
            }
        }
    }
}