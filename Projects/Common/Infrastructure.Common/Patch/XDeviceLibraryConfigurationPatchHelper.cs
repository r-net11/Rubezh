using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Common;
using FiresecAPI;
using Ionic.Zip;
using XFiresecAPI;

namespace Infrastructure.Common
{
    public static class XDeviceLibraryConfigurationPatchHelper
    {
        public static void Patch()
        {
            var emptyFileName = AppDataFolderHelper.GetFileInFolder("Empty", "Config.fscp");
            var fileName = Path.Combine(AppDataFolderHelper.GetServerAppDataPath(), "Config.fscp");

            var emptyZipFile = ZipFile.Read(emptyFileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });
            var xDeviceLibraryConfiguration = GetConfigurationFomZip(emptyZipFile, "XDeviceLibraryConfiguration.xml", typeof(XDeviceLibraryConfiguration));

            var zipFile = ZipFile.Read(fileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });
            AddConfigurationToZip(zipFile, xDeviceLibraryConfiguration, "XDeviceLibraryConfiguration.xml");
            zipFile.Save(fileName);
        }

        static VersionedConfiguration GetConfigurationFomZip(ZipFile zipFile, string fileName, Type type)
        {
            try
            {
                var configurationEntry = zipFile[fileName];
                if (configurationEntry != null)
                {
                    var configurationMemoryStream = new MemoryStream();
                    configurationEntry.Extract(configurationMemoryStream);
                    configurationMemoryStream.Position = 0;

                    var dataContractSerializer = new DataContractSerializer(type);
                    return (VersionedConfiguration)dataContractSerializer.ReadObject(configurationMemoryStream);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "ConfigActualizeHelper.GetFile " + fileName);
            }
            return null;
        }

        static void AddConfigurationToZip(ZipFile zipFile, VersionedConfiguration versionedConfiguration, string fileName)
        {
            var configuarationMemoryStream = ZipSerializeHelper.Serialize(versionedConfiguration);
            if (zipFile.Entries.Any(x => x.FileName == fileName))
            {
                zipFile.RemoveEntry(fileName);
            }
            configuarationMemoryStream.Position = 0;
            zipFile.AddEntry(fileName, configuarationMemoryStream);
        }
    }
}