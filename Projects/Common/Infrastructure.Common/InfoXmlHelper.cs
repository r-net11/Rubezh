using FiresecAPI.Models;

namespace Infrastructure.Common
{
    public static class InfoXmlHelper
    {
        public static void CreateInfoFile()
        {
            var configList = new ConfigurationsList();
            configList.Configurations.Add(new FiresecAPI.Models.Configuration() { MajorVersion = 1, MinorVersion = 1, Name = "DeviceLibrary" });
            configList.Configurations.Add(new FiresecAPI.Models.Configuration() { MajorVersion = 1, MinorVersion = 1, Name = "DeviceLibraryConfiguration" });
            configList.Configurations.Add(new FiresecAPI.Models.Configuration() { MajorVersion = 1, MinorVersion = 1, Name = "DriversConfiguration" });
            configList.Configurations.Add(new FiresecAPI.Models.Configuration() { MajorVersion = 1, MinorVersion = 1, Name = "PlansConfiguration" });
            configList.Configurations.Add(new FiresecAPI.Models.Configuration() { MajorVersion = 1, MinorVersion = 1, Name = "SystemConfiguration" });
            configList.Configurations.Add(new FiresecAPI.Models.Configuration() { MajorVersion = 1, MinorVersion = 1, Name = "XDeviceConfiguration" });
            configList.Configurations.Add(new FiresecAPI.Models.Configuration() { MajorVersion = 1, MinorVersion = 1, Name = "XDeviceLibraryConfiguration" });
            configList.Configurations.Add(new FiresecAPI.Models.Configuration() { MajorVersion = 1, MinorVersion = 1, Name = "SecurityConfiguration" });
            configList.Configurations.Add(new FiresecAPI.Models.Configuration() { MajorVersion = 1, MinorVersion = 1, Name = "DeviceConfiguration" });
            var ms = SerializeHelper.Serialize(configList);
            if (ConfigHelper.FromZip("Info.xml") == null)
                ConfigHelper.IntoZip("Info.xml", ms);
        }
    }
}
