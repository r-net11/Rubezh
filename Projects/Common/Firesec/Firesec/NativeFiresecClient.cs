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
	public class NativeFiresecClient : FS_Types.IFS_CallBack
	{
		private int _reguestId = 1;
		private Dispatcher _dispatcher;
		private Thread _taskThread;

		public NativeFiresecClient()
		{
			Dispatcher.CurrentDispatcher.ShutdownStarted += (s, e) =>
				{
					if (_taskThread != null)
						_taskThread.Abort();
					if (_dispatcher != null)
					{
						if (_connection != null)
						{
							Marshal.FinalReleaseComObject(_connection);
							_connection = null;
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

			_taskThread = new Thread(new ThreadStart(WorkTask));
			_taskThread.Start();
		}

		FS_Types.IFSC_Connection _connection;
		FS_Types.IFSC_Connection Connection
		{
			get
			{
				if (_connection == null)
					_connection = GetConnection("localhost", 211, "adm", "");
				return _connection;
			}
		}

		#region Operations
		public OperationResult<bool> Connect(string FS_Address, int FS_Port, string FS_Login, string FS_Password)
		{
			return SafeCall<bool>(() => { _connection = GetConnection(FS_Address, FS_Port, FS_Login, FS_Password); return true; }, "Connect");
		}

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
		public OperationResult<bool> ResetStates(string states)
		{
			return SafeCall<bool>(() => { Connection.ResetStates(states); return true; }, "ResetStates");
		}
		public OperationResult<string> ExecuteRuntimeDeviceMethod(string devicePath, string methodName, string parameters, ref int reguestId)
		{
			_reguestId++;
			reguestId = _reguestId;
			var result = SafeCall<string>(() => { return Connection.ExecuteRuntimeDeviceMethod(devicePath, methodName, parameters, _reguestId); }, "ExecuteRuntimeDeviceMethod");
			return result;
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
			Connection.ExecuteRuntimeDeviceMethod(devicePath, methodName, null, _reguestId++);
			return new OperationResult<bool>() { Result = true };
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
			return SafeCall<bool>(() => { Connection.IgoreListOperation(ConvertDeviceList(devicePaths), true); return true; }, "AddToIgnoreList");
		}
		public OperationResult<bool> RemoveFromIgnoreList(List<string> devicePaths)
		{
			return SafeCall<bool>(() => { Connection.IgoreListOperation(ConvertDeviceList(devicePaths), false); return true; }, "RemoveFromIgnoreList");
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
		#endregion

		#region Connection
		FS_Types.IFSC_Connection GetConnection(string FS_Address, int FS_Port, string FS_Login, string FS_Password)
		{
			SocketServerHelper.StartIfNotRunning();

			FS_Types.FSC_LIBRARY_CLASSClass library = new FS_Types.FSC_LIBRARY_CLASSClass();
			var serverInfo = new FS_Types.TFSC_ServerInfo()
			{
				ServerName = FS_Address,
				Port = FS_Port
			};

			try
			{
				ConnectionTimeoutEvent = new AutoResetEvent(false);
				ConnectionTimeoutThread = new Thread(new ThreadStart(OnConnectionTimeoutThread));
				ConnectionTimeoutThread.Start();

				FS_Types.IFSC_Connection connectoin = library.Connect2(FS_Login, FS_Password, serverInfo, this);
				ConnectionTimeoutEvent.Set();
				return connectoin;
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове NativeFiresecClient.GetConnection");
				SocketServerHelper.Restart();
				throw new Exception("Не удается подключиться к COM серверу Firesec");
			}
		}

		static Thread ConnectionTimeoutThread;
		static AutoResetEvent ConnectionTimeoutEvent;
		static void OnConnectionTimeoutThread()
		{
			if (!ConnectionTimeoutEvent.WaitOne(60000))
			{
				SocketServerHelper.Restart();
			}
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
					Logger.Error(e, "COMException " + e.Message + " при вызове " + methodName + " попытка " + i.ToString());
					resultData.Result = default(T);
					resultData.HasError = true;
					resultData.Error = e.Message;
					SocketServerHelper.StartIfNotRunning();
				}
				catch (Exception e)
				{
					Logger.Error(e, "Исключение при вызове NativeFiresecClient.SafeLoopCall попытка " + i.ToString());
					resultData.Result = default(T);
					resultData.HasError = true;
					resultData.Error = e.Message;
					SocketServerHelper.StartIfNotRunning();
				}
			}
			//SocketServerHelper.Restart();
			return resultData;
		}
		#endregion

		#region Callback
		public void NewEventsAvailable(int eventMask)
		{
			if (NewEventAvaliable != null)
				NewEventAvaliable(eventMask);
		}

		public event Action<int> NewEventAvaliable;
		public int IntContinueProgress = 1;

		public bool Progress(int Stage, string Comment, int PercentComplete, int BytesRW)
		{
			try
			{
				var continueProgress = IntContinueProgress == 1;
				IntContinueProgress = 1;
				ProcessProgress(Stage, Comment, PercentComplete, BytesRW);
				//return true;
				return continueProgress;
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове NativeFiresecClient.Progress");
				return false;
			}
		}
		#endregion

		#region Progress
		object locker = new object();
		Queue<ProgressData> taskQeue = new Queue<ProgressData>();
		public event Func<int, string, int, int, bool> ProgressEvent;

		public void ProcessProgress(int Stage, string Comment, int PercentComplete, int BytesRW)
		{
			lock (locker)
			{
				var progressData = new ProgressData()
				{
					Stage = Stage,
					Comment = Comment,
					PercentComplete = PercentComplete,
					BytesRW = BytesRW
				};

				taskQeue.Enqueue(progressData);
				Monitor.PulseAll(locker);
			}
		}

		void WorkTask()
		{
			while (true)
			{
				lock (locker)
				{
					while (taskQeue.Count == 0)
						Monitor.Wait(locker);

					ProgressData progressData = taskQeue.Dequeue();
					var result = OnProgress(progressData.Stage, progressData.Comment, progressData.PercentComplete, progressData.BytesRW);

					Interlocked.Exchange(ref IntContinueProgress, result ? 1 : 0);
				}
			}
		}

		bool OnProgress(int Stage, string Comment, int PercentComplete, int BytesRW)
		{
			if (ProgressEvent != null)
			{
				var result = ProgressEvent(Stage, Comment, PercentComplete, BytesRW);
                return result;
			}
            return true;

			bool stopProgress = (StopProgress == 1);
			StopProgress = 0;
			return stopProgress;
		}

		public int StopProgress;
		#endregion
	}
}