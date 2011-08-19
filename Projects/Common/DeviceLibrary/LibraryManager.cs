using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using DeviceLibrary.Models;
using Infrastructure.Common;

namespace DeviceLibrary
{
    public static class LibraryManager
    {
        public static List<Device> Devices { get; set; }

        static LibraryManager()
        {
            Devices = new List<Device>();
            GetDeviceLibraryConfiguration();
        }

        static void GetDeviceLibraryConfiguration()
        {
            var deviceLibraryConfiguration = new DeviceLibraryConfiguration();
            var dataContractSerializer = new DataContractSerializer(typeof(DeviceLibraryConfiguration));

            using (var fileStream = new FileStream(PathHelper.DeviceLibraryFileName, FileMode.Open))
            {
                deviceLibraryConfiguration = (DeviceLibraryConfiguration) dataContractSerializer.ReadObject(fileStream);
            }
            Devices = deviceLibraryConfiguration.Devices;
        }

        public static void SetDeviceLibraryConfiguration()
        {
            var deviceLibraryConfiguration = new DeviceLibraryConfiguration();
            deviceLibraryConfiguration.Devices = Devices;

            var dataContractSerializer = new DataContractSerializer(typeof(DeviceLibraryConfiguration));
            using (var fileStream = new FileStream(PathHelper.DeviceLibraryFileName, FileMode.Create))
            {
                dataContractSerializer.WriteObject(fileStream, deviceLibraryConfiguration);
            }
        }
    }
}