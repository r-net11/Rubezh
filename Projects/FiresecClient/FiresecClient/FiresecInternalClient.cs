using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.ServiceModel;
using System.Threading;
using FiresecServiceApi;
using System.Windows;

namespace FiresecClient
{
    public class FiresecInternalClient : IFiresecCallback
    {
        public static string CoreConfigString { get; set; }
        public static string CoreStateString { get; set; }
        public static string MetadataString { get; set; }
        public static string DeviceParametersString { get; set; }
        public static string JournalString { get; set; }

        DuplexChannelFactory<IFiresecService> _duplexChannelFactory;
        public static IFiresecService FiresecService;

        static object _locker = new object();
        static Thread _pingThread;

        public bool Start(string login, string password)
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.MaxBufferSize = Int32.MaxValue;
            binding.MaxReceivedMessageSize = Int32.MaxValue;
            binding.MaxBufferPoolSize = Int32.MaxValue;
            binding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
            EndpointAddress endpointAddress = new EndpointAddress("net.tcp://localhost:8000/FiresecService");
            _duplexChannelFactory = new DuplexChannelFactory<IFiresecService>(new InstanceContext(this), binding, endpointAddress);
            FiresecService = _duplexChannelFactory.CreateChannel();

            bool result = FiresecService.Connect(login, password);
            if (result == false)
                return false;

            _pingThread = new Thread(DoPing);
            _pingThread.Start();
            return true;
        }

        public void Stop()
        {
            _pingThread.Abort();
            //duplexChannelFactory.Close();
        }

        public void Subscribe()
        {
            FiresecService.Initialize();
        }

        public static Firesec.CoreConfig.config GetCoreConfig()
        {
            return Deserialize<Firesec.CoreConfig.config>(FiresecService.GetCoreConfig());
        }

        public static Firesec.CoreState.config GetCoreState()
        {
            return Deserialize<Firesec.CoreState.config>(FiresecService.GetCoreState());
        }

        public static Firesec.Metadata.config GetMetaData()
        {
            return Deserialize<Firesec.Metadata.config>(FiresecService.GetMetaData());
        }

        public static Firesec.DeviceParams.config GetDeviceParams()
        {
            return Deserialize<Firesec.DeviceParams.config>(FiresecService.GetCoreDeviceParams());
        }

        public static Firesec.ReadEvents.document ReadEvents(int fromId, int limit)
        {
            return Deserialize<Firesec.ReadEvents.document>(FiresecService.ReadEvents(fromId, limit));
        }

        public static void SetNewConfig(Firesec.CoreConfig.config coreConfig)
        {
            var stringConfig = Serialize<Firesec.CoreConfig.config>(coreConfig);
            FiresecService.SetNewConfig(stringConfig);
        }

        public static void SaveConfigToFile(Firesec.CoreConfig.config coreConfig, string fileName)
        {
            var stringConfig = Serialize<Firesec.CoreConfig.config>(coreConfig);

            FileStream fileStream = new FileStream(fileName, FileMode.Create);
            StreamWriter streamWriter = new StreamWriter(fileStream);
            streamWriter.Write(stringConfig);
            fileStream.Close();
        }

        public static Firesec.CoreConfig.config LoadConfigFromFile(string fileName)
        {
            FileStream fileStream = new FileStream(fileName, FileMode.Open);
            StreamReader streamReader = new StreamReader(fileStream);
            var stringConfig = streamReader.ReadToEnd();
            fileStream.Close();

            Firesec.CoreConfig.config coreConfig = Deserialize<Firesec.CoreConfig.config>(stringConfig);
            return coreConfig;
        }

        public static void DeviceWriteConfig(Firesec.CoreConfig.config coreConfig, string DevicePath)
        {
            FiresecService.DeviceWriteConfig(Serialize<Firesec.CoreConfig.config>(coreConfig), DevicePath);
        }

        public static void ResetStates(Firesec.CoreState.config coreState)
        {
            string stringCoreState = Serialize<Firesec.CoreState.config>(coreState);
            FiresecService.ResetStates(stringCoreState);
        }

        public static void ExecuteCommand(string devicePath, string methodName)
        {
            FiresecService.ExecuteCommand(devicePath, methodName);
        }

        public static void AddToIgnoreList(List<string> devicePaths)
        {
            FiresecService.AddToIgnoreList(devicePaths);
        }

        public static void RemoveFromIgnoreList(List<string> devicePaths)
        {
            FiresecService.RemoveFromIgnoreList(devicePaths);
        }

        public static void AddUserMessage(string message)
        {
            FiresecService.AddUserMessage(message);
        }

        public static Firesec.ZoneLogic.expr GetZoneLogic(string zoneLogicString)
        {
            try
            {
                return Deserialize<Firesec.ZoneLogic.expr>(zoneLogicString);
            }
            catch
            {
                return null;
            }
        }

        public static string SetZoneLogic(Firesec.ZoneLogic.expr zoneLogic)
        {
            return Serialize<Firesec.ZoneLogic.expr>(zoneLogic);
        }

        public static Firesec.Indicator.LEDProperties GetIndicatorLogic(string indicatorLogicString)
        {
            try
            {
                return Deserialize<Firesec.Indicator.LEDProperties>(indicatorLogicString);
            }
            catch
            {
                return null;
            }
        }

        public static string SetIndicatorLogic(Firesec.Indicator.LEDProperties indicatorLogic)
        {
            return Serialize<Firesec.Indicator.LEDProperties>(indicatorLogic);
        }

        public static void Ping()
        {
            FiresecService.Ping();
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
