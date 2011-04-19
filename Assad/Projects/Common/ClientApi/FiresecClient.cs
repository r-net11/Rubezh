using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.ServiceModel;
using ServiceApi;

namespace ClientApi
{
    public class FiresecClient : IFiresecCallback
    {
        public static string CoreConfigString { get; set; }
        public static string CoreStateString { get; set; }
        public static string MetadataString { get; set; }
        public static string DeviceParametersString { get; set; }
        public static string JournalString { get; set; }

        DuplexChannelFactory<IFiresecService> duplexChannelFactory;
        public static IFiresecService firesecService;

        static object locker = new object();

        public void Start()
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.MaxBufferSize = Int32.MaxValue;
            binding.MaxReceivedMessageSize = Int32.MaxValue;
            binding.MaxBufferPoolSize = Int32.MaxValue;
            binding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
            EndpointAddress endpointAddress = new EndpointAddress("net.tcp://localhost:8000/StateService");
            duplexChannelFactory = new DuplexChannelFactory<IFiresecService>(new InstanceContext(this), binding, endpointAddress);
            firesecService = duplexChannelFactory.CreateChannel();
        }

        public void Subscribe()
        {
            firesecService.Initialize();
        }

        public static Firesec.CoreConfig.config GetCoreConfig()
        {
            lock (locker)
            {
                CoreConfigString = firesecService.GetCoreConfig();
                byte[] bytes = Encoding.Default.GetBytes(CoreConfigString);
                MemoryStream memoryStream = new MemoryStream(bytes);

                XmlSerializer serializer = new XmlSerializer(typeof(Firesec.CoreConfig.config));
                Firesec.CoreConfig.config coreConfig = (Firesec.CoreConfig.config)serializer.Deserialize(memoryStream);
                memoryStream.Close();
                return coreConfig;
            }
        }

        public static Firesec.CoreState.config GetCoreState()
        {
            lock (locker)
            {
                CoreStateString = firesecService.GetCoreState();
                byte[] bytes = Encoding.Default.GetBytes(CoreStateString);
                MemoryStream memoryStream = new MemoryStream(bytes);

                XmlSerializer serializer = new XmlSerializer(typeof(Firesec.CoreState.config));
                Firesec.CoreState.config coreState = (Firesec.CoreState.config)serializer.Deserialize(memoryStream);
                memoryStream.Close();
                return coreState;
            }
        }

        public static Firesec.Metadata.config GetMetaData()
        {
            lock (locker)
            {
                MetadataString = firesecService.GetMetaData();
                byte[] bytes = Encoding.Default.GetBytes(MetadataString);
                MemoryStream memoryStream = new MemoryStream(bytes);

                XmlSerializer serializer = new XmlSerializer(typeof(Firesec.Metadata.config));
                Firesec.Metadata.config coreConfig = (Firesec.Metadata.config)serializer.Deserialize(memoryStream);
                memoryStream.Close();
                return coreConfig;
            }
        }

        public static Firesec.DeviceParams.config GetDeviceParams()
        {
            lock (locker)
            {
                DeviceParametersString = firesecService.GetCoreDeviceParams();
                byte[] bytes = Encoding.Default.GetBytes(DeviceParametersString);
                MemoryStream memoryStream = new MemoryStream(bytes);

                XmlSerializer serializer = new XmlSerializer(typeof(Firesec.DeviceParams.config));
                Firesec.DeviceParams.config deviceParamsConfig = (Firesec.DeviceParams.config)serializer.Deserialize(memoryStream);
                memoryStream.Close();
                return deviceParamsConfig;
            }
        }

        public static Firesec.ReadEvents.document ReadEvents(int fromId, int limit)
        {
            lock (locker)
            {
                JournalString = firesecService.ReadEvents(fromId, limit);
                byte[] bytes = Encoding.Default.GetBytes(JournalString);
                MemoryStream memoryStream = new MemoryStream(bytes);

                XmlSerializer serializer = new XmlSerializer(typeof(Firesec.ReadEvents.document));
                Firesec.ReadEvents.document journal = (Firesec.ReadEvents.document)serializer.Deserialize(memoryStream);
                memoryStream.Close();
                return journal;
            }
        }

        public static Firesec.ZoneLogic.expr GetZoneLogic(string zoneLogicString)
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

        public static string SetZoneLogic(Firesec.ZoneLogic.expr zoneLogic)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Firesec.ZoneLogic.expr));
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

        public static void SetNewConfig(Firesec.CoreConfig.config coreConfig)
        {
            lock (locker)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Firesec.CoreConfig.config));
                MemoryStream memoryStream = new MemoryStream();
                serializer.Serialize(memoryStream, coreConfig);
                byte[] bytes = memoryStream.ToArray();
                memoryStream.Close();
                string message = Encoding.UTF8.GetString(bytes);
                message = message.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");

                StreamWriter writer = new StreamWriter("SetNewConfig.xml");
                writer.Write(message);
                writer.Close();

                firesecService.SetNewConfig(message);
            }
        }

        public static void DeviceWriteConfig(Firesec.CoreConfig.config coreConfig, string DevicePath)
        {
            lock (locker)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Firesec.CoreConfig.config));
                MemoryStream memoryStream = new MemoryStream();
                serializer.Serialize(memoryStream, coreConfig);
                byte[] bytes = memoryStream.ToArray();
                memoryStream.Close();
                string message = Encoding.UTF8.GetString(bytes);
                message = message.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");

                firesecService.DeviceWriteConfig(message, DevicePath);
            }
        }

        public static void ResetStates(Firesec.CoreState.config coreState)
        {
            lock (locker)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Firesec.CoreState.config));
                MemoryStream memoryStream = new MemoryStream();
                serializer.Serialize(memoryStream, coreState);
                byte[] bytes = memoryStream.ToArray();
                memoryStream.Close();
                string message = Encoding.UTF8.GetString(bytes);
                message = message.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");

                firesecService.ResetStates(message);
            }
        }

        public void NewEventsAvailable(int eventMask, string obj)
        {
            OnNewEvent(eventMask, obj);
        }

        public static event Action<int, string> NewEvent;
        static void OnNewEvent(int eventMask, string obj)
        {
            if (NewEvent != null)
                NewEvent(eventMask, obj);
        }
    }
}
