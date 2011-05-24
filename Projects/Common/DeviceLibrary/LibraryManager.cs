using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using DeviceLibrary.Models;
using Firesec.Metadata;
using Infrastructure.Common;

namespace DeviceLibrary
{
    public static class LibraryManager
    {
        public static List<Device> Devices { get; set; }

        public static drvType[] Drivers { get; set; }

        static LibraryManager()
        {
            Load();
            LoadMetadata();
        }

        static void LoadMetadata()
        {
            var fileXml = new FileStream(PathHelper.MetadataFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            var serializer = new XmlSerializer(typeof(config));
            var metadata = (config)serializer.Deserialize(fileXml);
            fileXml.Close();
            Drivers = metadata.drv;
        }

        static void Load()
        {
            var fileStream = new FileStream(PathHelper.DeviceLibraryFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            var serializer = new XmlSerializer(typeof(DeviceManager));
            var deviceManager = (DeviceManager)serializer.Deserialize(fileStream);
            fileStream.Close();

            Devices = deviceManager.Devices;
        }

        public static void Save()
        {
            var deviceManager = new DeviceManager();
            deviceManager.Devices = Devices;

            var fileStream = new FileStream(PathHelper.DeviceLibraryFileName, FileMode.Create, FileAccess.Write, FileShare.Write);
            var serializer = new XmlSerializer(typeof(DeviceManager));
            serializer.Serialize(fileStream, deviceManager);
            fileStream.Close();
        }
    }
}
