using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Text;
using System.Runtime.Serialization;
using System.Windows.Controls;
using System.Threading;

namespace Firesec
{
    public static class NativeFiresecClient
    {
        static Control control;

        static NativeFiresecClient()
        {
            control = new Control();
        }

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

        public static FiresecOperationResult<bool> Connect(string login, string password)
        {
            return SafeCall<bool>(() => { _connectoin = GetConnection(login, password); return true; });
        }

        public static FiresecOperationResult<bool> Disconnect()
        {
            return SafeCall<bool>(() => { _connectoin = null; return true; });
        }

        public static FiresecOperationResult<string> GetCoreConfig()
        {
            return SafeCall<string>(() => { return ReadFromStream(Connectoin.GetCoreConfig()); });
        }

        public static FiresecOperationResult<string> GetPlans()
        {
            return SafeCall<string>(() => { return Connectoin.GetCoreAreasW(); });
        }

        public static FiresecOperationResult<string> GetMetadata()
        {
            return SafeCall<string>(() => { return ReadFromStream(Connectoin.GetMetaData()); });
        }

        public static FiresecOperationResult<string> GetCoreState()
        {
            return SafeCall<string>(() => { return ReadFromStream(Connectoin.GetCoreState()); });
        }

        public static FiresecOperationResult<string> GetCoreDeviceParams()
        {
            return SafeCall<string>(() => { return Connectoin.GetCoreDeviceParams(); });
        }

        public static FiresecOperationResult<string> ReadEvents(int fromId, int limit)
        {
            return SafeCall<string>(() => { return Connectoin.ReadEvents(fromId, limit); });
        }

        public static FiresecOperationResult<bool> SetNewConfig(string coreConfig)
        {
            return SafeCall<bool>(() => { Connectoin.SetNewConfig(coreConfig); return true; });
        }

        public static FiresecOperationResult<bool> ResetStates(string states)
        {
            return SafeCall<bool>(() => { Connectoin.ResetStates(states); return true; });
        }

        public static FiresecOperationResult<bool> ExecuteCommand(string devicePath, string methodName)
        {
            return SafeCall<bool>(() => { Connectoin.ExecuteRuntimeDeviceMethod(devicePath, methodName, null); return true; });
        }

        public static FiresecOperationResult<string> CheckHaspPresence()
        {
            return SafeCall<string>(() =>
            {
                string errorMessage = "";
                var result = Connectoin.CheckHaspPresence(out errorMessage);
                if (result)
                    return null;
                return errorMessage;
            });
        }

        public static FiresecOperationResult<bool> AddToIgnoreList(List<string> devicePaths)
        {
            return SafeCall<bool>(() => { Connectoin.IgoreListOperation(ConvertDeviceList(devicePaths), true); return true; });
        }

        public static FiresecOperationResult<bool> RemoveFromIgnoreList(List<string> devicePaths)
        {
            return SafeCall<bool>(() => { Connectoin.IgoreListOperation(ConvertDeviceList(devicePaths), false); return true; });
        }

        public static FiresecOperationResult<bool> AddUserMessage(string message)
        {
            return SafeCall<bool>(() => { Connectoin.StoreUserMessage(message); return true; });
        }

        public static FiresecOperationResult<bool> DeviceWriteConfig(string coreConfig, string devicePath)
        {
            return SafeCall<bool>(() => { Connectoin.DeviceWriteConfig(coreConfig, devicePath); return true; });
        }

        public static FiresecOperationResult<bool> DeviceSetPassword(string coreConfig, string devicePath, string password, int deviceUser)
        {
            return SafeCall<bool>(() => { Connectoin.DeviceSetPassword(coreConfig, devicePath, password, deviceUser); return true; });
        }

        public static FiresecOperationResult<bool> DeviceDatetimeSync(string coreConfig, string devicePath)
        {
            return SafeCall<bool>(() => { Connectoin.DeviceDatetimeSync(coreConfig, devicePath); return true; });
        }

        public static FiresecOperationResult<string> DeviceGetInformation(string coreConfig, string devicePath)
        {
            return SafeCall<string>(() => { return Connectoin.DeviceGetInformation(coreConfig, devicePath); });
        }

        public static FiresecOperationResult<string> DeviceGetSerialList(string coreConfig, string devicePath)
        {
            return SafeCall<string>(() => { return Connectoin.DeviceGetSerialList(coreConfig, devicePath); });
        }

        public static FiresecOperationResult<string> DeviceUpdateFirmware(string coreConfig, string devicePath, string fileName)
        {
            return SafeCall<string>(() => { return Connectoin.DeviceUpdateFirmware(coreConfig, devicePath, fileName); });
        }

        public static FiresecOperationResult<string> DeviceVerifyFirmwareVersion(string coreConfig, string devicePath, string fileName)
        {
            return SafeCall<string>(() => { return Connectoin.DeviceVerifyFirmwareVersion(coreConfig, devicePath, fileName); });
        }

        public static FiresecOperationResult<string> DeviceReadConfig(string coreConfig, string devicePath)
        {
            return SafeCall<string>(() => { return Connectoin.DeviceReadConfig(coreConfig, devicePath); });
        }

        public static FiresecOperationResult<string> DeviceReadEventLog(string coreConfig, string devicePath)
        {
            return SafeCall<string>(() => { return Connectoin.DeviceReadEventLog(coreConfig, devicePath, 0); });
        }

        public static FiresecOperationResult<string> DeviceAutoDetectChildren(string coreConfig, string devicePath, bool fastSearch)
        {
            return SafeCall<string>(() => { return Connectoin.DeviceAutoDetectChildren(coreConfig, devicePath, fastSearch); });
        }

        public static FiresecOperationResult<string> DeviceCustomFunctionList(string driverUID)
        {
            return SafeCall<string>(() => { return Connectoin.DeviceCustomFunctionList(driverUID); });
        }

        public static FiresecOperationResult<string> DeviceCustomFunctionExecute(string coreConfig, string devicePath, string functionName)
        {
            return SafeCall<string>(() => { return Connectoin.DeviceCustomFunctionExecute(coreConfig, devicePath, functionName); });
        }

        public static FiresecOperationResult<string> DeviceGetGuardUsersList(string coreConfig, string devicePath)
        {
            return SafeCall<string>(() => { return Connectoin.DeviceGetGuardUsersList(coreConfig, devicePath); });
        }

        public static FiresecOperationResult<bool> DeviceSetGuardUsersList(string coreConfig, string devicePath, string users)
        {
            return SafeCall<bool>(() => { Connectoin.DeviceSetGuardUsersList(coreConfig, devicePath, users); return true; });
        }

        public static FiresecOperationResult<string> DeviceGetMDS5Data(string coreConfig, string devicePath)
        {
            return SafeCall<string>(() => { return Connectoin.DeviceGetMDS5Data(coreConfig, devicePath); });
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

        static FiresecOperationResult<T> SafeCall<T>(Func<T> func)
        {
            return (FiresecOperationResult<T>)control.Dispatcher.Invoke
            (
                new Func<FiresecOperationResult<T>>
                (
                    () =>
                    {
                        return SafeLoopCall(func);
                    }
                )
            );
        }

        static FiresecOperationResult<T> SafeLoopCall<T>(Func<T> f)
        {
            var resultData = new FiresecOperationResult<T>();
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    var result = f();
                    resultData.Result = result;
                    return resultData;
                }
                catch (Exception e)
                {
                    resultData.Result = default(T);
                    resultData.HasError = true;
                    resultData.Error = e;
                }
            }
            return resultData;
        }

        static FiresecOperationResult<T> Temp<T>(Func<T> f)
        {
            return (FiresecOperationResult<T>)control.Dispatcher.Invoke(new Func<FiresecOperationResult<T>>(
                () =>
                {
                    var resultData = new FiresecOperationResult<T>();
                    for (int i = 0; i < 10; i++)
                    {
                        try
                        {
                            var result = f();
                            resultData.Result = result;
                            return resultData;
                        }
                        catch (Exception e)
                        {
                            resultData.Result = default(T);
                            resultData.HasError = true;
                            resultData.Error = e;
                        }
                    }
                    return resultData;
                }
                )
            );
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
    }
}