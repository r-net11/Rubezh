using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using Common;
using FiresecAPI;
using System.Diagnostics;

namespace Firesec
{
	public partial class NativeFiresecClient : FS_Types.IFS_CallBack
	{
		private int _reguestId = 1;
		private Dispatcher _dispatcher;

		public NativeFiresecClient()
		{
			Dispatcher.CurrentDispatcher.ShutdownStarted += (s, e) =>
				{
					if (_dispatcher != null)
					{
						if (Connection != null)
						{
                            StopPoll();
							StopThread();
							Marshal.FinalReleaseComObject(Connection);
							Connection = null;
							GC.Collect();
						}
						_dispatcher.InvokeShutdown();
					}
				};
			var dispatcherThread = new Thread(new ParameterizedThreadStart(obj =>
			{
				_dispatcher = Dispatcher.CurrentDispatcher;
				Dispatcher.Run();
				Debug.WriteLine("Native Dispatcher Stopped");
			}));
			dispatcherThread.SetApartmentState(ApartmentState.STA);
			dispatcherThread.IsBackground = true;
			dispatcherThread.Start();
			dispatcherThread.Join(100);
		}


		#region Operations
		public OperationResult<string> GetCoreConfig()
		{
			return SafeCall<string>(() => { return ReadFromStream(Connection.GetCoreConfig()); }, "GetCoreConfig");
		}
		public OperationResult<string> GetPlans()
		{
			return SafeCall<string>(() => { return Connection.GetCoreAreasW(); }, "GetPlans");
		}
		public OperationResult<string> GetMetadata()
		{
			return SafeCall<string>(() => { return ReadFromStream(Connection.GetMetaData()); }, "GetMetadata");
		}
		public OperationResult<string> GetCoreState()
		{
			return SafeCall<string>(() => { return ReadFromStream(Connection.GetCoreState()); }, "GetCoreState");
		}
		public OperationResult<string> GetCoreDeviceParams()
		{
			return SafeCall<string>(() => { return Connection.GetCoreDeviceParams(); }, "GetCoreDeviceParams");
		}
		public OperationResult<string> ReadEvents(int fromId, int limit)
		{
			return SafeCall<string>(() => { return Connection.ReadEvents(fromId, limit); }, "ReadEvents");
		}
		public OperationResult<bool> SetNewConfig(string coreConfig)
		{
			return SafeCall<bool>(() => { Connection.SetNewConfig(coreConfig); return true; }, "SetNewConfig");
		}
		public OperationResult<string> ExecuteRuntimeDeviceMethod(string devicePath, string methodName, string parameters, ref int reguestId)
		{
			_reguestId++;
			reguestId = _reguestId;
			return SafeCall<string>(() =>
			{
				return Connection.ExecuteRuntimeDeviceMethod(devicePath, methodName, parameters, _reguestId);
			}, "ExecuteRuntimeDeviceMethod");
		}
		public OperationResult<string> ExecuteRuntimeDeviceMethod(string devicePath, string methodName, string parameters)
		{
			return SafeCall<string>(() => { Connection.ExecuteRuntimeDeviceMethod(devicePath, methodName, parameters, _reguestId++); return null; }, "ExecuteRuntimeDeviceMethod");
		}
		public OperationResult<string> GetConfigurationParameters(string devicePath, int paramNo)
		{
			return SafeCall<string>(() =>
			{
				_reguestId += 1;
				var result1 = Connection.ExecuteRuntimeDeviceMethod(devicePath, "Device$ReadSimpleParam", paramNo.ToString(), _reguestId);
				Thread.Sleep(TimeSpan.FromSeconds(1));
				var result = Connection.ExecuteRuntimeDeviceMethod(devicePath, "StateConfigQueries", null, 0);
				return result;
			}, "GetConfigurationParameters");
		}
		public OperationResult<string> SetConfigurationParameters(string devicePath, string paramValues)
		{
			return SafeCall<string>(() =>
			{
				_reguestId += 1;
				var result = Connection.ExecuteRuntimeDeviceMethod(devicePath, "Device$WriteSimpleParam", paramValues, _reguestId);
				return result;
			}, "GetConfigurationParameters");
		}
		public OperationResult<bool> ExecuteCommand(string devicePath, string methodName)
		{
			return SafeCall<bool>(() =>
			{
				_reguestId += 1;
				var result = Connection.ExecuteRuntimeDeviceMethod(devicePath, methodName, "Test", _reguestId);
				return true;
			}, "ExecuteCommand");

			//Connection.ExecuteRuntimeDeviceMethod(devicePath, methodName, null, _reguestId++);
			//Connection.ExecuteRuntimeDeviceMethod(devicePath, methodName, "Test", _reguestId++);
			//return new OperationResult<bool>() { Result = true };
		}
		public OperationResult<bool> CheckHaspPresence()
		{
			return SafeCall<bool>(() =>
			{
				string errorMessage = "";
				try
				{
					var result = Connection.CheckHaspPresence(out errorMessage);
					return result;
				}
				catch (Exception e)
				{
					Logger.Error(e, "Исключение при вызове NativeFiresecClient.CheckHaspPresence");
					return true;
				}
			}, "CheckHaspPresence");
		}
        public OperationResult<bool> AddToIgnoreList(List<string> devicePaths)
        {
            AddTask(() =>
                {
                    SafeCall<bool>(() => { Connection.IgoreListOperation(ConvertDeviceList(devicePaths), true); return true; }, "AddToIgnoreList");
                });
            return new OperationResult<bool>();
        }
        public OperationResult<bool> RemoveFromIgnoreList(List<string> devicePaths)
        {
            AddTask(() =>
                {
                    SafeCall<bool>(() => { Connection.IgoreListOperation(ConvertDeviceList(devicePaths), false); return true; }, "RemoveFromIgnoreList");
                });
            return new OperationResult<bool>();
        }
        public OperationResult<bool> ResetStates(string states)
        {
            AddTask(() =>
                {
                    SafeCall<bool>(() =>
                    {
                        Connection.ResetStates(states);
                        return true;
                    }, "ResetStates");
                });
            return new OperationResult<bool>();
        }
        public void SetZoneGuard(string placeInTree, string localZoneNo)
        {
            AddTask(() =>
            {
                SafeCall<bool>(() => { int reguestId = 0; ExecuteRuntimeDeviceMethod(placeInTree, "SetZoneToGuard", localZoneNo, ref reguestId); return true; }, "SetZoneGuard");
            });
        }
        public void UnSetZoneGuard(string placeInTree, string localZoneNo)
        {
            AddTask(() =>
            {
                SafeCall<bool>(() => { int reguestId = 0; ExecuteRuntimeDeviceMethod(placeInTree, "UnSetZoneFromGuard", localZoneNo, ref reguestId); return true; }, "UnSetZoneGuard");
            });
        }
		public OperationResult<bool> AddUserMessage(string message)
		{
			return SafeCall<bool>(() => { Connection.StoreUserMessage(message); return true; }, "AddUserMessage");
		}
		public OperationResult<bool> DeviceWriteConfig(string coreConfig, string devicePath)
		{
			return SafeCall<bool>(() => { Connection.DeviceWriteConfig(coreConfig, devicePath); return true; }, "DeviceWriteConfig");
		}
		public OperationResult<bool> DeviceSetPassword(string coreConfig, string devicePath, string password, int deviceUser)
		{
			return SafeCall<bool>(() => { Connection.DeviceSetPassword(coreConfig, devicePath, password, deviceUser); return true; }, "DeviceSetPassword");
		}
		public OperationResult<bool> DeviceDatetimeSync(string coreConfig, string devicePath)
		{
			return SafeCall<bool>(() => { Connection.DeviceDatetimeSync(coreConfig, devicePath); return true; }, "DeviceDatetimeSync");
		}
		public OperationResult<string> DeviceGetInformation(string coreConfig, string devicePath)
		{
			return SafeCall<string>(() => { return Connection.DeviceGetInformation(coreConfig, devicePath); }, "DeviceGetInformation");
		}
		public OperationResult<string> DeviceGetSerialList(string coreConfig, string devicePath)
		{
			return SafeCall<string>(() => { return Connection.DeviceGetSerialList(coreConfig, devicePath); }, "DeviceGetSerialList");
		}
		public OperationResult<string> DeviceUpdateFirmware(string coreConfig, string devicePath, string fileName)
		{
			return SafeCall<string>(() => { return Connection.DeviceUpdateFirmware(coreConfig, devicePath, fileName); }, "DeviceUpdateFirmware");
		}
		public OperationResult<string> DeviceVerifyFirmwareVersion(string coreConfig, string devicePath, string fileName)
		{
			return SafeCall<string>(() => { return Connection.DeviceVerifyFirmwareVersion(coreConfig, devicePath, fileName); }, "DeviceVerifyFirmwareVersion");
		}
		public OperationResult<string> DeviceReadConfig(string coreConfig, string devicePath)
		{
			return SafeCall<string>(() => { return Connection.DeviceReadConfig(coreConfig, devicePath); }, "DeviceReadConfig");
		}
		public OperationResult<string> DeviceReadEventLog(string coreConfig, string devicePath)
		{
			return SafeCall<string>(() => { return Connection.DeviceReadEventLog(coreConfig, devicePath, 0); }, "DeviceReadEventLog");
		}
		public OperationResult<string> DeviceAutoDetectChildren(string coreConfig, string devicePath, bool fastSearch)
		{
			return SafeCall<string>(() => { return Connection.DeviceAutoDetectChildren(coreConfig, devicePath, fastSearch); }, "DeviceAutoDetectChildren");
		}
		public OperationResult<string> DeviceCustomFunctionList(string driverUID)
		{
			return SafeCall<string>(() => { return Connection.DeviceCustomFunctionList(driverUID); }, "DeviceCustomFunctionList");
		}
		public OperationResult<string> DeviceCustomFunctionExecute(string coreConfig, string devicePath, string functionName)
		{
			return SafeCall<string>(() => { return Connection.DeviceCustomFunctionExecute(coreConfig, devicePath, functionName); }, "DeviceCustomFunctionExecute");
		}
		public OperationResult<string> DeviceGetGuardUsersList(string coreConfig, string devicePath)
		{
			return SafeCall<string>(() => { return Connection.DeviceGetGuardUsersList(coreConfig, devicePath); }, "DeviceGetGuardUsersList");
		}
		public OperationResult<bool> DeviceSetGuardUsersList(string coreConfig, string devicePath, string users)
		{
			return SafeCall<bool>(() => { Connection.DeviceSetGuardUsersList(coreConfig, devicePath, users); return true; }, "DeviceSetGuardUsersList");
		}
		public OperationResult<string> DeviceGetMDS5Data(string coreConfig, string devicePath)
		{
			return SafeCall<string>(() => { return Connection.DeviceGetMDS5Data(coreConfig, devicePath); }, "DeviceGetMDS5Data");
		}

		string ConvertDeviceList(List<string> devicePaths)
		{
			var devicePatsString = new StringBuilder();
			foreach (string device in devicePaths)
			{
				devicePatsString.AppendLine(device);
			}
			return devicePatsString.ToString().TrimEnd();
		}

		string ReadFromStream(mscoree.IStream stream)
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
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове NativeFiresecClient.ReadFromStream");
			}

			return stringBuilder.ToString();
		}
		#endregion

		#region SafeCall
		OperationResult<T> SafeCall<T>(Func<T> func, string methodName)
		{
			var safeCallResult = (OperationResult<T>)_dispatcher.Invoke
			(
				new Func<OperationResult<T>>
				(
					() =>
					{
						return SafeLoopCall(func, methodName);
					}
				)
			);
			return safeCallResult;
		}

		OperationResult<T> SafeLoopCall<T>(Func<T> f, string methodName)
		{
			var resultData = new OperationResult<T>();
			for (int i = 0; i < 3; i++)
			{
				try
				{
					var result = f();
					resultData.Result = result;
					resultData.HasError = false;
					resultData.Error = null;
					return resultData;
				}
				catch (COMException e)
				{
					string exceptionText = "COMException " + e.Message + " при вызове " + methodName + " попытка " + i.ToString();
					Trace.WriteLine(exceptionText);
					Logger.Error(exceptionText);
					resultData.Result = default(T);
					resultData.HasError = true;
					resultData.Error = e.Message;
					SocketServerHelper.StartIfNotRunning();
				}
				catch (Exception e)
				{
					string exceptionText = "Исключение при вызове NativeFiresecClient.SafeLoopCall попытка " + i.ToString();
					Trace.WriteLine(exceptionText);
					Logger.Error(e, exceptionText);
					resultData.Result = default(T);
					resultData.HasError = true;
					resultData.Error = e.Message;
					SocketServerHelper.StartIfNotRunning();
				}
			}
			return resultData;
		}
		#endregion

		#region Callback
		public void NewEventsAvailable(int eventMask)
		{
			bool evmNewEvents = ((eventMask & 1) == 1);
			bool evmStateChanged = ((eventMask & 2) == 2);
			bool evmConfigChanged = ((eventMask & 4) == 4);
			bool evmDeviceParamsUpdated = ((eventMask & 8) == 8);
			bool evmPong = ((eventMask & 16) == 16);
			bool evmDatabaseChanged = ((eventMask & 32) == 32);
			bool evmReportsChanged = ((eventMask & 64) == 64);
			bool evmSoundsChanged = ((eventMask & 128) == 128);
			bool evmLibraryChanged = ((eventMask & 256) == 256);
			bool evmPing = ((eventMask & 512) == 512);
			bool evmIgnoreListChanged = ((eventMask & 1024) == 1024);
			bool evmEventViewChanged = ((eventMask & 2048) == 2048);

			IsSuspending = true;
			try
			{
				if (evmStateChanged)
				{
					var result = SafeLoopCall<string>(() => { return ReadFromStream(Connection.GetCoreState()); }, "GetCoreState");
					if (result != null && result.Result != null)
					{
						var coreState = ConvertResultData<Firesec.Models.CoreState.config>(result);
						StateChanged(coreState.Result);
					}
				}

				if (evmDeviceParamsUpdated)
				{
					var result = SafeLoopCall<string>(() => { return Connection.GetCoreDeviceParams(); }, "GetCoreDeviceParams");
					if (result != null && result.Result != null)
					{
						var coreParameters = ConvertResultData<Firesec.Models.DeviceParameters.config>(result);
						ParametersChanged(coreParameters.Result);
					}
				}

				if (evmNewEvents)
				{
					;
				}
			}
			catch { ;}
			finally
			{
				IsSuspending = false;
			}

			if (NewEventAvaliable != null)
				NewEventAvaliable(eventMask);
		}

		OperationResult<T> ConvertResultData<T>(OperationResult<string> result)
		{
			var resultData = new OperationResult<T>();
			resultData.HasError = result.HasError;
			resultData.Error = result.Error;
			if (result.HasError == false)
				resultData.Result = SerializerHelper.Deserialize<T>(result.Result);
			return resultData;
		}

		public bool Progress(int Stage, string Comment, int PercentComplete, int BytesRW)
		{
			try
			{
				if (ProgressEvent != null)
				{
					return ProgressEvent(Stage, Comment, PercentComplete, BytesRW);
				}
				return true;
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове NativeFiresecClient.Progress");
				return false;
			}
		}

		public event Action<Firesec.Models.CoreState.config> StateChanged;
		public event Action<Firesec.Models.DeviceParameters.config> ParametersChanged;
		public event Action<int> NewEventAvaliable;
		public event Func<int, string, int, int, bool> ProgressEvent;
		#endregion
	}
}