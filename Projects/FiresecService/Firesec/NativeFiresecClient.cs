using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using Common;

namespace Firesec
{
	public class NativeFiresecClient : FS_Types.IFS_CallBack
	{
		Control control;

		public NativeFiresecClient()
		{
			control = new Control();
			var thread = new Thread(new ThreadStart(WorkTask));
			thread.Start();
		}

		FS_Types.IFSC_Connection _connectoin;
		FS_Types.IFSC_Connection Connectoin
		{
			get
			{
				if (_connectoin == null)
					_connectoin = GetConnection("adm", "");
				return _connectoin;
			}
		}

		public FiresecOperationResult<bool> Connect(string login, string password)
		{
			return SafeCall<bool>(() => { _connectoin = GetConnection(login, password); return true; });
		}

		public FiresecOperationResult<string> GetCoreConfig()
		{
			return SafeCall<string>(() => { return ReadFromStream(Connectoin.GetCoreConfig()); });
		}

		public FiresecOperationResult<string> GetPlans()
		{
			return SafeCall<string>(() => { return Connectoin.GetCoreAreasW(); });
		}

		public FiresecOperationResult<string> GetMetadata()
		{
			return SafeCall<string>(() => { return ReadFromStream(Connectoin.GetMetaData()); });
		}

		public FiresecOperationResult<string> GetCoreState()
		{
			return SafeCall<string>(() => { return ReadFromStream(Connectoin.GetCoreState()); });
		}

		public FiresecOperationResult<string> GetCoreDeviceParams()
		{
			return SafeCall<string>(() => { return Connectoin.GetCoreDeviceParams(); });
		}

		public FiresecOperationResult<string> ReadEvents(int fromId, int limit)
		{
			return SafeCall<string>(() => { return Connectoin.ReadEvents(fromId, limit); });
		}

		public FiresecOperationResult<bool> SetNewConfig(string coreConfig)
		{
			return SafeCall<bool>(() => { Connectoin.SetNewConfig(coreConfig); return true; });
		}

		public FiresecOperationResult<bool> ResetStates(string states)
		{
			return SafeCall<bool>(() => { Connectoin.ResetStates(states); return true; });
		}

		int ReguestId = 1;

		public FiresecOperationResult<string> ExecuteRuntimeDeviceMethod(string devicePath, string methodName, string parameters, ref int reguestId)
		{
			ReguestId++;
			reguestId = ReguestId;
			var result = SafeCall<string>(() => { return Connectoin.ExecuteRuntimeDeviceMethod(devicePath, methodName, parameters, ReguestId); });
			return result;
		}

		public FiresecOperationResult<string> ExecuteRuntimeDeviceMethod(string devicePath, string methodName, string parameters)
		{
			return SafeCall<string>(() => { Connectoin.ExecuteRuntimeDeviceMethod(devicePath, methodName, parameters, ReguestId++); return null; });
		}

		public FiresecOperationResult<string> GetConfigurationParameters(string devicePath, int paramNo)
		{
			return SafeCall<string>(() =>
			{
				ReguestId += 1;
				var result1 = Connectoin.ExecuteRuntimeDeviceMethod(devicePath, "Device$ReadSimpleParam", paramNo.ToString(), ReguestId);
				Thread.Sleep(TimeSpan.FromSeconds(1));
				var result = Connectoin.ExecuteRuntimeDeviceMethod(devicePath, "StateConfigQueries", null, 0);
				return result;
			});
		}

		public FiresecOperationResult<string> SetConfigurationParameters(string devicePath, string paramValues)
		{
			return SafeCall<string>(() =>
			{
				ReguestId += 1;
				var result = Connectoin.ExecuteRuntimeDeviceMethod(devicePath, "Device$WriteSimpleParam", paramValues, ReguestId);
				return result;
			});
		}

		public FiresecOperationResult<bool> ExecuteCommand(string devicePath, string methodName)
		{
			Connectoin.ExecuteRuntimeDeviceMethod(devicePath, methodName, null, ReguestId++);
			return null;
		}

		public FiresecOperationResult<bool> CheckHaspPresence()
		{
			return SafeCall<bool>(() =>
			{
				string errorMessage = "";
				var result = Connectoin.CheckHaspPresence(out errorMessage);
				if (result != true)
					throw new Exception(errorMessage);
				return result;
			});
		}

		public FiresecOperationResult<bool> AddToIgnoreList(List<string> devicePaths)
		{
			return SafeCall<bool>(() => { Connectoin.IgoreListOperation(ConvertDeviceList(devicePaths), true); return true; });
		}

		public FiresecOperationResult<bool> RemoveFromIgnoreList(List<string> devicePaths)
		{
			return SafeCall<bool>(() => { Connectoin.IgoreListOperation(ConvertDeviceList(devicePaths), false); return true; });
		}

		public FiresecOperationResult<bool> AddUserMessage(string message)
		{
			return SafeCall<bool>(() => { Connectoin.StoreUserMessage(message); return true; });
		}

		public FiresecOperationResult<bool> DeviceWriteConfig(string coreConfig, string devicePath)
		{
			return SafeCall<bool>(() => { Connectoin.DeviceWriteConfig(coreConfig, devicePath); return true; });
		}

		public FiresecOperationResult<bool> DeviceSetPassword(string coreConfig, string devicePath, string password, int deviceUser)
		{
			return SafeCall<bool>(() => { Connectoin.DeviceSetPassword(coreConfig, devicePath, password, deviceUser); return true; });
		}

		public FiresecOperationResult<bool> DeviceDatetimeSync(string coreConfig, string devicePath)
		{
			return SafeCall<bool>(() => { Connectoin.DeviceDatetimeSync(coreConfig, devicePath); return true; });
		}

		public FiresecOperationResult<string> DeviceGetInformation(string coreConfig, string devicePath)
		{
			return SafeCall<string>(() => { return Connectoin.DeviceGetInformation(coreConfig, devicePath); });
		}

		public FiresecOperationResult<string> DeviceGetSerialList(string coreConfig, string devicePath)
		{
			return SafeCall<string>(() => { return Connectoin.DeviceGetSerialList(coreConfig, devicePath); });
		}

		public FiresecOperationResult<string> DeviceUpdateFirmware(string coreConfig, string devicePath, string fileName)
		{
			return SafeCall<string>(() => { return Connectoin.DeviceUpdateFirmware(coreConfig, devicePath, fileName); });
		}

		public FiresecOperationResult<string> DeviceVerifyFirmwareVersion(string coreConfig, string devicePath, string fileName)
		{
			return SafeCall<string>(() => { return Connectoin.DeviceVerifyFirmwareVersion(coreConfig, devicePath, fileName); });
		}

		public FiresecOperationResult<string> DeviceReadConfig(string coreConfig, string devicePath)
		{
			return SafeCall<string>(() => { return Connectoin.DeviceReadConfig(coreConfig, devicePath); });
		}

		public FiresecOperationResult<string> DeviceReadEventLog(string coreConfig, string devicePath)
		{
			return SafeCall<string>(() => { return Connectoin.DeviceReadEventLog(coreConfig, devicePath, 0); });
		}

		public FiresecOperationResult<string> DeviceAutoDetectChildren(string coreConfig, string devicePath, bool fastSearch)
		{
			return SafeCall<string>(() => { return Connectoin.DeviceAutoDetectChildren(coreConfig, devicePath, fastSearch); });
		}

		public FiresecOperationResult<string> DeviceCustomFunctionList(string driverUID)
		{
			return SafeCall<string>(() => { return Connectoin.DeviceCustomFunctionList(driverUID); });
		}

		public FiresecOperationResult<string> DeviceCustomFunctionExecute(string coreConfig, string devicePath, string functionName)
		{
			return SafeCall<string>(() => { return Connectoin.DeviceCustomFunctionExecute(coreConfig, devicePath, functionName); });
		}

		public FiresecOperationResult<string> DeviceGetGuardUsersList(string coreConfig, string devicePath)
		{
			return SafeCall<string>(() => { return Connectoin.DeviceGetGuardUsersList(coreConfig, devicePath); });
		}

		public FiresecOperationResult<bool> DeviceSetGuardUsersList(string coreConfig, string devicePath, string users)
		{
			return SafeCall<bool>(() => { Connectoin.DeviceSetGuardUsersList(coreConfig, devicePath, users); return true; });
		}

		public FiresecOperationResult<string> DeviceGetMDS5Data(string coreConfig, string devicePath)
		{
			return SafeCall<string>(() => { return Connectoin.DeviceGetMDS5Data(coreConfig, devicePath); });
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

		#region Connection
		FS_Types.IFSC_Connection GetConnection(string login, string password)
		{
			FS_Types.FSC_LIBRARY_CLASSClass library = new FS_Types.FSC_LIBRARY_CLASSClass();

			var serverInfo = new FS_Types.TFSC_ServerInfo()
			{
				Port = 211,
				ServerName = "localhost"
			};

			try
			{
				FS_Types.IFSC_Connection connectoin = library.Connect3(login, password, serverInfo, this, false);
				//FS_Types.IFSC_Connection connectoin = library.Connect2(login, password, serverInfo, this);
				return connectoin;
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове NativeFiresecClient.GetConnection");
				throw new Exception("Не удается подключиться к COM серверу Firesec");
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
		FiresecOperationResult<T> SafeCall<T>(Func<T> func)
		{
			var safeCallResult = (FiresecOperationResult<T>)control.Dispatcher.Invoke
			(
				new Func<FiresecOperationResult<T>>
				(
					() =>
					{
						return SafeLoopCall(func);
					}
				)
			);
			return safeCallResult;
		}

		FiresecOperationResult<T> SafeLoopCall<T>(Func<T> f)
		{
			var resultData = new FiresecOperationResult<T>();
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
				catch (Exception e)
				{
					Logger.Error(e, "Исключение при вызове NativeFiresecClient.SafeLoopCall попытка " + i.ToString());
					resultData.Result = default(T);
					resultData.HasError = true;
					resultData.Error = e;
				}
			}
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
				return ProgressEvent(Stage, Comment, PercentComplete, BytesRW);
			return true;
		}
		#endregion
	}
}