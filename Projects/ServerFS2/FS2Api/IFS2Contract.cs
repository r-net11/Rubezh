using System;
using System.Collections.Generic;
using System.ServiceModel;
using FiresecAPI;
using FiresecAPI.Models;

namespace FS2Api
{
	[ServiceContract]
	public interface IFS2Contract
	{
		#region Main
		[OperationContract]
		List<FS2Callbac> Poll(Guid clientUID);

		[OperationContract]
		void CancelPoll(Guid clientUID);

		[OperationContract]
		void CancelProgress();
		#endregion

		[OperationContract]
		OperationResult<bool> SetConfiguration(DeviceConfiguration deviceConfiguration);

		[OperationContract]
		OperationResult<DeviceConfiguration> GetConfiguration();

		[OperationContract]
		OperationResult<bool> WriteConfiguration(Guid deviceUID);

		[OperationContract]
		OperationResult<string> GetInfo(Guid deviceUID);

		[OperationContract]
		OperationResult<List<FS2JournalItem>> ReadJournal(Guid deviceUID);

		[OperationContract]
		OperationResult<DeviceConfiguration> ReadConfiguration(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SynchronizeTime(Guid deviceUID);
	}
}