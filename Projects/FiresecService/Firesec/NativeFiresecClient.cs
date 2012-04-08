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
            try
            {
                return ReadFromStream(Connectoin.GetCoreState());
            }
            catch
            {
                return null;
            }
        }

        public static string GetCoreDeviceParams()
        {
            try
            {
                return Connectoin.GetCoreDeviceParams();
            }
            catch
            {
                return null;
            }
        }

        public static string ReadEvents(int fromId, int limit)
        {
            try
            {
                return Connectoin.ReadEvents(fromId, limit);
            }
            catch
            {
                return null;
            }
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

        public static string CheckHaspPresence()
        {
            string errorMessage = "";
            var result = Connectoin.CheckHaspPresence(out errorMessage);
            if (result)
                return null;
            return errorMessage;
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

        public static string DeviceWriteConfig(string coreConfig, string devicePath)
        {
            try
            {
                Connectoin.DeviceWriteConfig(coreConfig, devicePath);
                return null;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static bool DeviceSetPassword(string coreConfig, string devicePath, string password, int deviceUser)
        {
            try
            {
                Connectoin.DeviceSetPassword(coreConfig, devicePath, password, deviceUser);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool DeviceDatetimeSync(string coreConfig, string devicePath)
        {
            try
            {
                Connectoin.DeviceDatetimeSync(coreConfig, devicePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string DeviceGetInformation(string coreConfig, string devicePath)
        {
            try
            {
                var result = Connectoin.DeviceGetInformation(coreConfig, devicePath);
                return result;
            }
            catch (Exception e) { return null; }
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
            var result = Connectoin.DeviceVerifyFirmwareVersion(coreConfig, devicePath, fileName);
            return result;
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

        public static string DeviceGetGuardUsersList(string coreConfig, string devicePath)
        {
            return Connectoin.DeviceGetGuardUsersList(coreConfig, devicePath);
        }

        public static string DeviceSetGuardUsersList(string coreConfig, string devicePath, string users)
        {
            Connectoin.DeviceSetGuardUsersList(coreConfig, devicePath, users);
            return null;
        }

        public static string DeviceGetMDS5Data(string coreConfig, string devicePath)
        {
            return Connectoin.DeviceGetMDS5Data(coreConfig, devicePath);
        }

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