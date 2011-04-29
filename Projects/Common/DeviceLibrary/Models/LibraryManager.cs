using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using Firesec.Metadata;

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
            var file_xml = new FileStream(ResourceHelper.metadata_xml, FileMode.Open, FileAccess.Read, FileShare.Read);
            var serializer = new XmlSerializer(typeof(config));
            var metadata = (config)serializer.Deserialize(file_xml);
            file_xml.Close();
            Drivers = metadata.drv;
        }

        static void Load()
        {
            FileStream fileStream = new FileStream(ResourceHelper.deviceLibrary_xml, FileMode.Open, FileAccess.Read, FileShare.Read);
            XmlSerializer serializer = new XmlSerializer(typeof(DeviceManager));
            DeviceManager deviceManager = (DeviceManager)serializer.Deserialize(fileStream);
            fileStream.Close();

            Devices = deviceManager.Devices;
        }

        public static void Save()
        {
            DeviceManager deviceManager = new DeviceManager();
            deviceManager.Devices = Devices;

            FileStream fileStream = new FileStream(ResourceHelper.deviceLibrary_xml, FileMode.Create, FileAccess.Write, FileShare.Write);
            XmlSerializer serializer = new XmlSerializer(typeof(DeviceManager));
            serializer.Serialize(fileStream, deviceManager);
            fileStream.Close();
        }
    }
}
