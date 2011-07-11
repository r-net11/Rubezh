using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting;
using System.Diagnostics;

namespace Firesec
{
    public class NativeFiresecClient
    {
        static FS_Types.IFSC_Connection connectoin;
        static FS_Types.IFSC_Connection Connectoin
        {
            get
            {
                if (connectoin == null)
                {
                    connectoin = GetConnection("adm", "");
                }
                return connectoin;
            }
        }

        public static void Connect(string login, string password)
        {
            //connectoin = GetConnection(login, password);
        }

        public static string GetCoreConfig()
        {
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

        public static void DeviceWriteConfig(string coreConfig, string DevicePath)
        {
            Connectoin.DeviceWriteConfig(coreConfig, DevicePath);
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
            //FS_Types.FSC_LIBRARY_CLASSClass library = (FS_Types.FSC_LIBRARY_CLASSClass)objectHandle.Unwrap();
            FS_Types.FSC_LIBRARY_CLASS library = (FS_Types.FSC_LIBRARY_CLASS)objectHandle.Unwrap();
            FS_Types.TFSC_ServerInfo serverInfo = new FS_Types.TFSC_ServerInfo();
            serverInfo.Port = 211;
            serverInfo.ServerName = "localhost";

            NotificationCallBack notificationCallBack = new NotificationCallBack();

            FS_Types.IFSC_Connection connectoin = library.Connect2(login, password, serverInfo, notificationCallBack);
            //FS_Types.IFSC_Connection connectoin = library.Connect2("adm", "", serverInfo, notificationCallBack);

            return connectoin;
        }

        static string ReadFromStream(mscoree.IStream stream)
        {
            string message = "";

            unsafe
            {
                while (true)
                {
                    byte* unsafeBytes = stackalloc byte[1024];
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
                    message += Encoding.Default.GetString(bytes);
                }
            }
            return message;
        }
    }
}
