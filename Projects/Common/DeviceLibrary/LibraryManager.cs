using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using DeviceLibrary.Models;
using Infrastructure.Common;

namespace DeviceLibrary
{
    public static class LibraryManager
    {
        public static List<Device> Devices { get; set; }

        static LibraryManager()
        {
            Load();
        }

        static void Load()
        {
            using (var fileStream =
                new FileStream(PathHelper.DeviceLibraryFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var serializer = new XmlSerializer(typeof(DeviceManager));
                var deviceManager = (DeviceManager) serializer.Deserialize(fileStream);
                Devices = deviceManager.Devices;
            }
        }

        public static void Save()
        {
            var deviceManager = new DeviceManager();
            deviceManager.Devices = Devices;

            using (var fileStream =
                new FileStream(PathHelper.DeviceLibraryFileName, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                var serializer = new XmlSerializer(typeof(DeviceManager));
                serializer.Serialize(fileStream, deviceManager);
            }
        }
    }
}