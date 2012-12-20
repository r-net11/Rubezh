using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using Common;

namespace MuliclientAPI
{
    public static class MulticlientConfigurationHelper
    {
        public static MulticlientConfiguration LoadConfiguration(string password)
        {
            try
            {
                EncryptHelper.DecryptFile("Configuration.xml", "TempConfiguration.xml", password);

                var memStream = new MemoryStream();
                using (var fileStream = new FileStream("TempConfiguration.xml", FileMode.Open))
                {
                    memStream.SetLength(fileStream.Length);
                    fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
                }
                File.Delete("TempConfiguration.xml");
                var dataContractSerializer = new DataContractSerializer(typeof(MulticlientConfiguration));
                var configuration = (MulticlientConfiguration)dataContractSerializer.ReadObject(memStream);
                if (configuration == null)
                    return new MulticlientConfiguration();
                return configuration;
            }
            catch (Exception e)
            {
                Logger.Error(e, "MulticlientConfigurationHelper.LoadData");
            }
            return new MulticlientConfiguration();
        }

        public static void SaveConfiguration(MulticlientConfiguration configuration, string password)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    var dataContractSerializer = new DataContractSerializer(typeof(MulticlientConfiguration));
                    dataContractSerializer.WriteObject(memoryStream, configuration);

                    using (var fileStream = new FileStream("TempConfiguration.xml", FileMode.Create))
                    {
                        fileStream.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Position);
                    }
                    EncryptHelper.EncryptFile("TempConfiguration.xml", "Configuration.xml", password);
                    File.Delete("TempConfiguration.xml");
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "MulticlientConfigurationHelper.SaveData");
            }
        }
    }
}