using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS2Api;
using System.ServiceModel;
using FiresecAPI;
using FiresecAPI.Models;
using Common;

namespace ServerFS2.Service
{
	[ServiceBehavior(MaxItemsInObjectGraph = Int32.MaxValue, UseSynchronizationContext = false,
	InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class FS2Contracr : IFS2Contract
	{
		[OperationContract]
		public List<FS2Callbac> Poll(Guid clientUID)
		{
			throw new NotImplementedException();
		}

		[OperationContract]
		public void CancelPoll(Guid clientUID)
		{
			throw new NotImplementedException();
		}

		[OperationContract]
		public void CancelProgress()
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> SetConfiguration(DeviceConfiguration deviceConfiguration)
		{
			throw new NotImplementedException();
		}

		public OperationResult<DeviceConfiguration> GetConfiguration()
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> WriteConfiguration(Guid deviceUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<string> GetInfo(Guid deviceUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<List<FS2JournalItem>> ReadJournal(Guid deviceUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<DeviceConfiguration> ReadConfiguration(Guid deviceUID)
		{
			throw new NotImplementedException();
		}

		public OperationResult<bool> SynchronizeTime(Guid deviceUID)
		{
			return SafeCall<bool>(() =>
			{
				var device = ConfigurationManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
				ServerHelper.SynchronizeTime(device);
				return true;
			}, "DeviceDatetimeSync");
		}

		OperationResult<T> SafeCall<T>(Func<T> func, string methodName)
		{
			var resultData = new OperationResult<T>();
			try
			{
				var result = func();
				resultData.Result = result;
				resultData.HasError = false;
				resultData.Error = null;
				return resultData;
			}
			catch (Exception e)
			{
				string exceptionText = "Exception " + e.Message + " при вызове " + methodName;
				Logger.Error(e, exceptionText);
				resultData.Result = default(T);
				resultData.HasError = true;
				resultData.Error = e.Message;
			}
			return resultData;
		}
	}
}