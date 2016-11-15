using Common;
using StrazhAPI;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Infrastructure.Common
{
    public class ZipSerializeHelper
    {
        public static MemoryStream Serialize<T>(T configuration)
            where T : VersionedConfiguration
        {
            configuration.BeforeSave();
            configuration.Version = new ConfigurationVersion { MajorVersion = 1, MinorVersion = 1 };
            var memoryStream = new MemoryStream();

            var xmlSerializer = XmlSerializer.FromTypes(new[] { configuration.GetType() })[0]; //new XmlSerializer(configuration.GetType());
            xmlSerializer.Serialize(memoryStream, configuration);
            return memoryStream;
        }

        public static T DeSerialize<T>(Stream stream)
             where T : VersionedConfiguration, new()
        {
            var xmlSerializer = XmlSerializer.FromTypes(new[] { typeof(T) })[0]; //new XmlSerializer(typeof(T));
            var configuration = (T)xmlSerializer.Deserialize(stream);
            configuration.ValidateVersion();
            configuration.AfterLoad();
            return configuration;
        }

        public static bool Serialize<T>(T configuration, string fileName, bool useXml)
            where T : VersionedConfiguration
        {
            try
            {
                if (useXml)
                {
                    var xmlSerializer = XmlSerializer.FromTypes(new[] { configuration.GetType() })[0];
                    using (var fileStream = new FileStream(fileName, FileMode.Create))
                    {
                        xmlSerializer.Serialize(fileStream, configuration);
                    }
                }
                else
                {
                    var dataContractSerializer = new DataContractSerializer(configuration.GetType());
                    using (var fileStream = new FileStream(fileName, FileMode.Create))
                    {
                        dataContractSerializer.WriteObject(fileStream, configuration);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "ZipSerializeHelper.Serialize");
                return false;
            }
            return true;
        }

        public static T DeSerialize<T>(string fileName, bool useXml)
             where T : VersionedConfiguration, new()
        {
            try
            {
                using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    T configuration;
                    if (useXml)
                    {
                        var xmlSerializer = XmlSerializer.FromTypes(new[] { typeof(T) })[0]; //new XmlSerializer(typeof(T));
                        configuration = (T)xmlSerializer.Deserialize(fileStream);
                    }
                    else
                    {
                        var dataContractSerializer = new DataContractSerializer(typeof(T));
                        try
                        {
                            configuration = (T)dataContractSerializer.ReadObject(fileStream);
                        }
                        catch (SerializationException)
                        {
                            dataContractSerializer = new DataContractSerializer(typeof(T), null, int.MaxValue, false, false, null, new StrazhDataContractResolver());
                            var xml = System.IO.File.ReadAllText(fileName);
                            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
                            var xmlReader = XmlReader.Create(memoryStream);
                            XmlReader strazhXmlReader = new StrazhXmlReader(xmlReader);
                            configuration = (T)dataContractSerializer.ReadObject(strazhXmlReader);
                        }
                    }
                    fileStream.Close();
                    configuration.ValidateVersion();
                    configuration.AfterLoad();
                    return configuration;
                }
            }
            catch (Exception e)
            {
                Logger.Error("ZipSerializeHelper.DeSerialize " + fileName + " " + e.Message);
                return new T();
            }
        }
    }
}