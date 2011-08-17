using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Text;

namespace Firesec
{
    public static class NativeFiresecClient
    {
        static FS_Types.IFSC_Connection _connectoin;
        static FS_Types.IFSC_Connection Connectoin
        {
            get
            {
                if (_connectoin == null)
                {
                    _connectoin = GetConnection("adm", "");
                }
                return _connectoin;
            }
        }

        public static bool Connect(string login, string password)
        {
            try
            {
                _connectoin = GetConnection(login, password);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static void Disconnect()
        {
            _connectoin = null;
        }

        public static string GetCoreConfig()
        {
            //return Connectoin.GetCoreConfigW();
            mscoree.IStream stream = Connectoin.GetCoreConfig();
            return ReadFromStream(stream);
        }

        public static string GetCoreState()
        {
            mscoree.IStream stream = Connectoin.GetCoreState();
            return ReadFromStream(stream);
        }

        public static string GetMetaData()
        {
            mscoree.IStream stream = Connectoin.GetMetaData();
            return ReadFromStream(stream);
        }

        public static string GetCoreDeviceParams()
        {
            return Connectoin.GetCoreDeviceParams();
        }

        public static string ReadEvents(int fromId, int limit)
        {
            return Connectoin.ReadEvents(fromId, limit);
        }

        public static void SetNewConfig(string coreConfig)
        {
            Connectoin.SetNewConfig(coreConfig);
        }

        public static void DeviceWriteConfig(string coreConfig, string devicePath)
        {
            Connectoin.DeviceWriteConfig(coreConfig, devicePath);
        }

        public static void ResetStates(string states)
        {
            Connectoin.ResetStates(states);
        }

        public static void ExecuteCommand(string devicePath, string methodName)
        {
            Connectoin.ExecuteRuntimeDeviceMethod(devicePath, methodName, null);
        }

        static string ConvertDeviceList(List<string> devicePaths)
        {
            string separator = "\r\n";
            string devices = "";
            foreach (string device in devicePaths)
            {
                devices += device + separator;
            }
            if (devices.EndsWith(separator))
                devices = devices.Remove(devices.LastIndexOf(separator));

            return devices;
        }

        public static void AddToIgnoreList(List<string> devicePaths)
        {
            string devices = ConvertDeviceList(devicePaths);
            Connectoin.IgoreListOperation(devices, true);
        }

        public static void RemoveFromIgnoreList(List<string> devicePaths)
        {
            string devices = ConvertDeviceList(devicePaths);
            Connectoin.IgoreListOperation(devices, false);
        }

        public static void AddUserMessage(string message)
        {
            Connectoin.StoreUserMessage(message);
        }

        static FS_Types.IFSC_Connection GetConnection(string login, string password)
        {
            ObjectHandle objectHandle = Activator.CreateComInstanceFrom("Interop.FS_Types.dll", "FS_Types.FSC_LIBRARY_CLASSClass");
            FS_Types.FSC_LIBRARY_CLASSClass library = (FS_Types.FSC_LIBRARY_CLASSClass) objectHandle.Unwrap();
            //FS_Types.FSC_LIBRARY_CLASS library = (FS_Types.FSC_LIBRARY_CLASS)objectHandle.Unwrap();
            FS_Types.TFSC_ServerInfo serverInfo = new FS_Types.TFSC_ServerInfo();
            serverInfo.Port = 211;
            serverInfo.ServerName = "localhost";

            NotificationCallBack notificationCallBack = new NotificationCallBack();

            FS_Types.IFSC_Connection connectoin;
            try
            {
                connectoin = library.Connect2(login, password, serverInfo, notificationCallBack);
            }
            catch
            {
                throw new Exception();
            }
            return connectoin;
        }

        static string ReadFromStream(mscoree.IStream stream)
        {
            StringBuilder stringBuilder = new StringBuilder();

            try
            {
                unsafe
                {
                    byte* unsafeBytes = stackalloc byte[1024];
                    while (true)
                    {
                        IntPtr _intPtr = new IntPtr(unsafeBytes);
                        uint bytesRead = 0;
                        stream.Read(_intPtr, 1024, out bytesRead);
                        if (bytesRead == 0)
                            break;

                        byte[] bytes = new byte[bytesRead];
                        for (int i = 0; i < bytesRead; i++)
                        {
                            bytes[i] = unsafeBytes[i];
                        }
                        string part = Encoding.Default.GetString(bytes);
                        stringBuilder.Append(part);
                    }
                }
            }
            catch (Exception)
            {
            }

            return stringBuilder.ToString();
        }
    }
}