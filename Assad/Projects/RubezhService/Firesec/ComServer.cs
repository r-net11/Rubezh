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
    public class ComServer
    {
        public static CoreConfig.config GetCoreConfig()
        {
            string coreConfigString = NativeComServer.GetCoreConfig();
            byte[] bytes = Encoding.Default.GetBytes(coreConfigString);
            MemoryStream memoryStream = new MemoryStream(bytes);

            XmlSerializer serializer = new XmlSerializer(typeof(CoreConfig.config));
            CoreConfig.config coreConfig = (CoreConfig.config)serializer.Deserialize(memoryStream);
            memoryStream.Close();
            return coreConfig;
        }

        public static CoreState.config GetCoreState()
        {
            string coreStateString = NativeComServer.GetCoreState();
            byte[] bytes = Encoding.Default.GetBytes(coreStateString);
            MemoryStream memoryStream = new MemoryStream(bytes);

            XmlSerializer serializer = new XmlSerializer(typeof(CoreState.config));
            CoreState.config coreConfig = (CoreState.config)serializer.Deserialize(memoryStream);
            memoryStream.Close();
            return coreConfig;
        }

        public static Metadata.config GetMetaData()
        {
            string metadataString = NativeComServer.GetMetaData();
            byte[] bytes = Encoding.Default.GetBytes(metadataString);
            MemoryStream memoryStream = new MemoryStream(bytes);

            XmlSerializer serializer = new XmlSerializer(typeof(Metadata.config));
            Metadata.config coreConfig = (Metadata.config)serializer.Deserialize(memoryStream);
            memoryStream.Close();
            return coreConfig;
        }

        public static DeviceParams.config GetDeviceParams()
        {
            string DeviceParamsString = NativeComServer.GetCoreDeviceParams();
            byte[] bytes = Encoding.Default.GetBytes(DeviceParamsString);
            MemoryStream memoryStream = new MemoryStream(bytes);

            XmlSerializer serializer = new XmlSerializer(typeof(DeviceParams.config));
            DeviceParams.config deviceParamsConfig = (DeviceParams.config)serializer.Deserialize(memoryStream);
            memoryStream.Close();
            return deviceParamsConfig;
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

            NativeComServer.SetNewConfig(message);
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

            NativeComServer.DeviceWriteConfig(message, DevicePath);
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

            NativeComServer.ResetStates(message);
        }
    }
}
