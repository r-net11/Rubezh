using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting;
using System.IO;
using System.Xml.Serialization;
using System.Windows;

namespace Firesec
{
    public class FiresecClient
    {
        public static string CoreConfigString { get; set; }
        public static string CoreStateString { get; set; }
        public static string MetadataString { get; set; }
        public static string DeviceParametersString { get; set; }
        public static string JournalString { get; set; }

        public static CoreConfig.config GetCoreConfig()
        {
            CoreConfigString = NativeFiresecClient.GetCoreConfig();
            byte[] bytes = Encoding.Default.GetBytes(CoreConfigString);
            MemoryStream memoryStream = new MemoryStream(bytes);

            XmlSerializer serializer = new XmlSerializer(typeof(CoreConfig.config));
            CoreConfig.config coreConfig = (CoreConfig.config)serializer.Deserialize(memoryStream);
            memoryStream.Close();
            return coreConfig;
        }

        public static CoreState.config GetCoreState()
        {
            CoreStateString = NativeFiresecClient.GetCoreState();
            byte[] bytes = Encoding.Default.GetBytes(CoreStateString);
            MemoryStream memoryStream = new MemoryStream(bytes);

            XmlSerializer serializer = new XmlSerializer(typeof(CoreState.config));
            CoreState.config coreConfig = (CoreState.config)serializer.Deserialize(memoryStream);
            memoryStream.Close();
            return coreConfig;
        }

        public static Metadata.config GetMetaData()
        {
            MetadataString = NativeFiresecClient.GetMetaData();
            byte[] bytes = Encoding.Default.GetBytes(MetadataString);
            MemoryStream memoryStream = new MemoryStream(bytes);

            XmlSerializer serializer = new XmlSerializer(typeof(Metadata.config));
            Metadata.config coreConfig = (Metadata.config)serializer.Deserialize(memoryStream);
            memoryStream.Close();
            return coreConfig;
        }

        public static DeviceParams.config GetDeviceParams()
        {
            DeviceParametersString = NativeFiresecClient.GetCoreDeviceParams();
            byte[] bytes = Encoding.Default.GetBytes(DeviceParametersString);
            MemoryStream memoryStream = new MemoryStream(bytes);

            XmlSerializer serializer = new XmlSerializer(typeof(DeviceParams.config));
            DeviceParams.config deviceParamsConfig = (DeviceParams.config)serializer.Deserialize(memoryStream);
            memoryStream.Close();
            return deviceParamsConfig;
        }

        public static ReadEvents.document ReadEvents(int fromId, int limit)
        {
            JournalString = NativeFiresecClient.ReadEvents(fromId, limit);
            byte[] bytes = Encoding.Default.GetBytes(JournalString);
            MemoryStream memoryStream = new MemoryStream(bytes);

            XmlSerializer serializer = new XmlSerializer(typeof(ReadEvents.document));
            ReadEvents.document journal = (ReadEvents.document)serializer.Deserialize(memoryStream);
            memoryStream.Close();
            return journal;
        }

        public static ZoneLogic.expr GetZoneLogic(string zoneLogicString)
        {
            try
            {
                byte[] bytes = Encoding.Default.GetBytes(zoneLogicString);
                MemoryStream memoryStream = new MemoryStream(bytes);

                XmlSerializer serializer = new XmlSerializer(typeof(Firesec.ZoneLogic.expr));
                Firesec.ZoneLogic.expr zoneLogic = (Firesec.ZoneLogic.expr)serializer.Deserialize(memoryStream);
                memoryStream.Close();
                return zoneLogic;
            }
            catch
            {
            }
            return null;
        }

        public static string SetZoneLogic(ZoneLogic.expr zoneLogic)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ZoneLogic.expr));
                MemoryStream memoryStream = new MemoryStream();
                serializer.Serialize(memoryStream, zoneLogic);
                byte[] bytes = memoryStream.ToArray();
                memoryStream.Close();
                //string message = Encoding.GetEncoding("windows-1251").GetString(bytes);
                string message = Encoding.Default.GetString(bytes);
                message = message.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");

                message = message.Replace("<?xml version=\"1.0\"?>", "<?xml version=\"1.0\" encoding=\"windows-1251\"?>");
                //message = message.Replace("\n", "").Replace("\r", "");
                message = message.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;").Replace("=", "&#061;").Replace("/", "&#047;");

                //message = message.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
                return message;
            }
            catch
            {
            }
            return null;
        }

        public static void SetNewConfig(CoreConfig.config coreConfig)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CoreConfig.config));
            MemoryStream memoryStream = new MemoryStream();
            serializer.Serialize(memoryStream, coreConfig);
            byte[] bytes = memoryStream.ToArray();
            memoryStream.Close();
            string message = Encoding.UTF8.GetString(bytes);
            message = message.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");

            StreamWriter writer = new StreamWriter("SetNewConfig.xml");
            writer.Write(message);
            writer.Close();

            NativeFiresecClient.SetNewConfig(message);
        }

        public static void DeviceWriteConfig(CoreConfig.config coreConfig, string DevicePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CoreConfig.config));
            MemoryStream memoryStream = new MemoryStream();
            serializer.Serialize(memoryStream, coreConfig);
            byte[] bytes = memoryStream.ToArray();
            memoryStream.Close();
            string message = Encoding.UTF8.GetString(bytes);
            message = message.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");

            NativeFiresecClient.DeviceWriteConfig(message, DevicePath);
        }

        public static void ResetStates(CoreState.config coreState)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CoreState.config));
            MemoryStream memoryStream = new MemoryStream();
            serializer.Serialize(memoryStream, coreState);
            byte[] bytes = memoryStream.ToArray();
            memoryStream.Close();
            string message = Encoding.UTF8.GetString(bytes);
            message = message.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");

            NativeFiresecClient.ResetStates(message);
        }
    }
}
