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
                    _connectoin = GetConnection("adm", "");
                return _connectoin;
            }
        }

        public static bool Connect(string login, string password)
        {
            try { _connectoin = GetConnection(login, password); }
            catch { return false; }
            return true;
        }

        public static void Disconnect()
        {
            _connectoin = null;
        }

        public static string GetCoreConfig()
        {
            return ReadFromStream(Connectoin.GetCoreConfig());
        }

        public static string GetPlans()
        {
            return Connectoin.GetCoreAreasW();
        }

        public static string GetMetadata()
        {
            return ReadFromStream(Connectoin.GetMetaData());
        }

        public static string GetCoreState()
        {
            return ReadFromStream(Connectoin.GetCoreState());
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
            var devicePatsString = new StringBuilder();
            foreach (string device in devicePaths)
            {
                devicePatsString.AppendLine(device);
            }

            return devicePatsString.ToString().TrimEnd();
        }

        public static void AddToIgnoreList(List<string> devicePaths)
        {
            Connectoin.IgoreListOperation(ConvertDeviceList(devicePaths), true);
        }

        public static void RemoveFromIgnoreList(List<string> devicePaths)
        {
            Connectoin.IgoreListOperation(ConvertDeviceList(devicePaths), false);
        }

        public static void AddUserMessage(string message)
        {
            Connectoin.StoreUserMessage(message);
        }

        //*********************************************************************

        public static void DeviceWriteConfig(string coreConfig, string devicePath)
        {
            Connectoin.DeviceWriteConfig(coreConfig, devicePath);
        }

        public static void DeviceSetPassword(string coreConfig, string devicePath, string password, int deviceUser)
        {
            Connectoin.DeviceSetPassword(coreConfig, devicePath, password, deviceUser);
        }

        public static void DeviceDatetimeSync(string coreConfig, string devicePath)
        {
            Connectoin.DeviceDatetimeSync(coreConfig, devicePath);
        }

        public static void DeviceRestart(string coreConfig, string devicePath)
        {
            Connectoin.DeviceRestart(coreConfig, devicePath);
        }

        public static string DeviceGetInformation(string coreConfig, string devicePath)
        {
            return Connectoin.DeviceGetInformation(coreConfig, devicePath);
        }

        public static string DeviceGetSerialList(string coreConfig, string devicePath)
        {
            return Connectoin.DeviceGetSerialList(coreConfig, devicePath);
        }

        public static string DeviceUpdateFirmware(string coreConfig, string devicePath, string fileName)
        {
            return Connectoin.DeviceUpdateFirmware(coreConfig, devicePath, fileName);
        }

        public static string DeviceVerifyFirmwareVersion(string coreConfig, string devicePath, string fileName)
        {
            return Connectoin.DeviceVerifyFirmwareVersion(coreConfig, devicePath, fileName);
        }

        public static string DeviceReadConfig(string coreConfig, string devicePath)
        {
            return Connectoin.DeviceReadConfig(coreConfig, devicePath);
        }

        public static string DeviceReadEventLog(string coreConfig, string devicePath)
        {
            try
            {
                return Connectoin.DeviceReadEventLog(coreConfig, devicePath, 0);
            }
            catch
            {
                return null;
            }
        }

        public static string DeviceAutoDetectChildren(string coreConfig, string devicePath, bool fastSearch)
        {
            try
            {
                return Connectoin.DeviceAutoDetectChildren(coreConfig, devicePath, fastSearch);
            }
            catch
            {
                return null;
            }
        }

        public static string DeviceCustomFunctionList(string driverUID)
        {
            return Connectoin.DeviceCustomFunctionList(driverUID);
        }

        public static string DeviceCustomFunctionExecute(string coreConfig, string devicePath, string functionName)
        {
            return Connectoin.DeviceCustomFunctionExecute(coreConfig, devicePath, functionName);
        }

        //*********************************************************************

        static FS_Types.IFSC_Connection GetConnection(string login, string password)
        {
            ObjectHandle objectHandle = Activator.CreateComInstanceFrom("Interop.FS_Types.dll", "FS_Types.FSC_LIBRARY_CLASSClass");
            //FS_Types.FSC_LIBRARY_CLASSClass library = (FS_Types.FSC_LIBRARY_CLASSClass)objectHandle.Unwrap();
            FS_Types.FSC_LIBRARY_CLASS library = (FS_Types.FSC_LIBRARY_CLASS)objectHandle.Unwrap();
            FS_Types.TFSC_ServerInfo serverInfo = new FS_Types.TFSC_ServerInfo()
            {
                Port = 211,
                ServerName = "localhost"
            };

            FS_Types.IFSC_Connection connectoin;
            try
            {
                connectoin = library.Connect2(login, password, serverInfo, new NotificationCallBack());
            }
            catch
            {
                throw new Exception();
            }
            return connectoin;
        }

        static string ReadFromStream(mscoree.IStream stream)
        {
            var stringBuilder = new StringBuilder();
            try
            {
                unsafe
                {
                    byte* unsafeBytes = stackalloc byte[1024];
                    while (true)
                    {
                        var _intPtr = new IntPtr(unsafeBytes);
                        uint bytesRead = 0;
                        stream.Read(_intPtr, 1024, out bytesRead);
                        if (bytesRead == 0)
                            break;

                        var bytes = new byte[bytesRead];
                        for (int i = 0; i < bytesRead; ++i)
                        {
                            bytes[i] = unsafeBytes[i];
                        }
                        stringBuilder.Append(Encoding.Default.GetString(bytes));
                    }
                }
            }
            catch { }

            return stringBuilder.ToString();
        }
    }
}