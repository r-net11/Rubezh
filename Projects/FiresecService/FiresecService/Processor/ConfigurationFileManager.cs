using System;
using System.IO;
using System.Runtime.Serialization;
using FiresecAPI.Models;

namespace FiresecService
{
    public static class ConfigurationFileManager
    {
        readonly static string SystemConfigurationFileName = "SystemConfiguration.xml";
        readonly static string DeviceLibraryConfigurationFileName = "DeviceLibrary.xml";
        readonly static string PlansConfigurationFileName = "PlansConfiguration.xml";
        readonly static string DeviceConfigurationFileName = "DeviceConfiguration.xml";
        readonly static string SecurityConfigurationFileName = "SecurityConfiguration.xml";

        public static string ConfigurationDirectory(string FileNameOrDirectory)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration", FileNameOrDirectory);
        }

        public static DeviceConfiguration GetDeviceConfiguration()
        {
            try
            {
                var dataContractSerializer = new DataContractSerializer(typeof(DeviceConfiguration));
                using (var fileStream = new FileStream(ConfigurationDirectory(DeviceConfigurationFileName), FileMode.Open))
                {
                    return (DeviceConfiguration) dataContractSerializer.ReadObject(fileStream);
                }
            }
            catch
            {
                return new DeviceConfiguration();
            }
        }

        public static void SetDeviceConfiguration(DeviceConfiguration deviceConfiguration)
        {
            var dataContractSerializer = new DataContractSerializer(typeof(DeviceConfiguration));
            Directory.CreateDirectory("Configuration");
            using (var fileStream = new FileStream(ConfigurationDirectory(DeviceConfigurationFileName), FileMode.Create))
            {
                dataContractSerializer.WriteObject(fileStream, deviceConfiguration);
            }
        }

        public static SystemConfiguration GetSystemConfiguration()
        {
            try
            {
                var dataContractSerializer = new DataContractSerializer(typeof(SystemConfiguration));
                using (var fileStream = new FileStream(ConfigurationDirectory(SystemConfigurationFileName), FileMode.Open))
                {
                    return (SystemConfiguration) dataContractSerializer.ReadObject(fileStream);
                }
            }
            catch
            {
                return new SystemConfiguration();
            }
        }

        public static void SetSystemConfiguration(SystemConfiguration systemConfiguration)
        {
            var dataContractSerializer = new DataContractSerializer(typeof(SystemConfiguration));
            using (var fileStream = new FileStream(ConfigurationDirectory(SystemConfigurationFileName), FileMode.Create))
            {
                dataContractSerializer.WriteObject(fileStream, systemConfiguration);
            }
        }

        public static LibraryConfiguration GetLibraryConfiguration()
        {
            try
            {
                var dataContractSerializer = new DataContractSerializer(typeof(LibraryConfiguration));
                using (var fileStream = new FileStream(ConfigurationDirectory(DeviceLibraryConfigurationFileName), FileMode.Open))
                {
                    return (LibraryConfiguration) dataContractSerializer.ReadObject(fileStream);
                }
            }
            catch
            {
                return new LibraryConfiguration();
            }
        }

        public static void SetLibraryConfiguration(LibraryConfiguration libraryConfiguration)
        {
            var dataContractSerializer = new DataContractSerializer(typeof(LibraryConfiguration));
            using (var fileStream = new FileStream(ConfigurationDirectory(DeviceLibraryConfigurationFileName), FileMode.Create))
            {
                dataContractSerializer.WriteObject(fileStream, libraryConfiguration);
            }
        }

        public static PlansConfiguration GetPlansConfiguration()
        {
            try
            {
                var dataContractSerializer = new DataContractSerializer(typeof(PlansConfiguration));
                using (var fileStream = new FileStream(ConfigurationDirectory(PlansConfigurationFileName), FileMode.Open))
                {
                    return (PlansConfiguration)dataContractSerializer.ReadObject(fileStream);
                }
            }
            catch
            {
                return new PlansConfiguration();
            }
        }

        public static void SetPlansConfiguration(PlansConfiguration plansConfiguration)
        {
            var dataContractSerializer = new DataContractSerializer(typeof(PlansConfiguration));
            using (var fileStream = new FileStream(ConfigurationDirectory(PlansConfigurationFileName), FileMode.Create))
            {
                dataContractSerializer.WriteObject(fileStream, plansConfiguration);
            }
        }

        public static SecurityConfiguration GetSecurityConfiguration()
        {
            try
            {
                var dataContractSerializer = new DataContractSerializer(typeof(SecurityConfiguration));
                using (var fileStream = new FileStream(ConfigurationDirectory(SecurityConfigurationFileName), FileMode.Open))
                {
                    return (SecurityConfiguration) dataContractSerializer.ReadObject(fileStream);
                }
            }
            catch
            {
                return new SecurityConfiguration();
            }
        }

        public static void SetSecurityConfiguration(SecurityConfiguration securityConfiguration)
        {
            var dataContractSerializer = new DataContractSerializer(typeof(SecurityConfiguration));
            using (var fileStream = new FileStream(ConfigurationDirectory(SecurityConfigurationFileName), FileMode.Create))
            {
                dataContractSerializer.WriteObject(fileStream, securityConfiguration);
            }
        }
    }
}