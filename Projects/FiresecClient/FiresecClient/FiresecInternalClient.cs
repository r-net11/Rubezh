using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.ServiceModel;
using System.Threading;
using FiresecServiceApi;

namespace FiresecClient
{
    public class FiresecInternalClient : IFiresecCallback
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
            EndpointAddress endpointAddress = new EndpointAddress("net.tcp://localhost:8000/FiresecService");
            duplexChannelFactory = new DuplexChannelFactory<IFiresecService>(new InstanceContext(this), binding, endpointAddress);
            firesecService = duplexChannelFactory.CreateChannel();

            new Thread(DoPing).Start();
        }

        public void Stop()
        {
            //duplexChannelFactory.Close();
        }

        public void Subscribe()
        {
            firesecService.Initialize();
        }

        public static Firesec.CoreConfig.config GetCoreConfig()
        {
            lock (locker)
            {
                return Deserialize<Firesec.CoreConfig.config>(firesecService.GetCoreConfig());
            }
        }

        public static Firesec.CoreState.config GetCoreState()
        {
            lock (locker)
            {
                return Deserialize<Firesec.CoreState.config>(firesecService.GetCoreState());
            }
        }

        public static Firesec.Metadata.config GetMetaData()
        {
            lock (locker)
            {
                return Deserialize<Firesec.Metadata.config>(firesecService.GetMetaData());
            }
        }

        public static Firesec.DeviceParams.config GetDeviceParams()
        {
            lock (locker)
            {
                return Deserialize<Firesec.DeviceParams.config>(firesecService.GetCoreDeviceParams());
            }
        }

        public static Firesec.ReadEvents.document ReadEvents(int fromId, int limit)
        {
            lock (locker)
            {
                return Deserialize<Firesec.ReadEvents.document>(firesecService.ReadEvents(fromId, limit));
            }
        }

        public static void SetNewConfig(Firesec.CoreConfig.config coreConfig)
        {
            lock (locker)
            {
                firesecService.SetNewConfig(Serialize<Firesec.CoreConfig.config>(coreConfig));
            }
        }

        public static void DeviceWriteConfig(Firesec.CoreConfig.config coreConfig, string DevicePath)
        {
            lock (locker)
            {
                firesecService.DeviceWriteConfig(Serialize<Firesec.CoreConfig.config>(coreConfig), DevicePath);
            }
        }

        public static void ResetStates(Firesec.CoreState.config coreState)
        {
            lock (locker)
            {
                firesecService.ResetStates(Serialize<Firesec.CoreState.config>(coreState));
            }
        }

        public static Firesec.ZoneLogic.expr GetZoneLogic(string zoneLogicString)
        {
            return Deserialize<Firesec.ZoneLogic.expr>(zoneLogicString);
        }

        public static string SetZoneLogic(Firesec.ZoneLogic.expr zoneLogic)
        {
            return Serialize<Firesec.ZoneLogic.expr>(zoneLogic);
        }

        public static void Ping()
        {
            lock (locker)
            {
                firesecService.Ping();
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

        public static void DoPing()
        {
            Thread.Sleep(TimeSpan.FromMinutes(1));
            Ping();
        }

        #region Helpers

        public static T Deserialize<T>(string input)
        {
            byte[] bytes = Encoding.Default.GetBytes(input);
            MemoryStream memoryStream = new MemoryStream(bytes);

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            T output = (T)serializer.Deserialize(memoryStream);
            memoryStream.Close();
            return output;
        }

        public static string Serialize<T>(T input)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            MemoryStream memoryStream = new MemoryStream();
            serializer.Serialize(memoryStream, input);
            byte[] bytes = memoryStream.ToArray();
            memoryStream.Close();
            string output = Encoding.UTF8.GetString(bytes);
            output = output.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");
            return output;
        }

        #endregion
    }
}
