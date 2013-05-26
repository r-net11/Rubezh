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
		[OperationContract]
		OperationResult<bool> SetConfiguration(DeviceConfiguration deviceConfiguration);

		[OperationContract]
		OperationResult<DeviceConfiguration> GetConfiguration();

		[OperationContract]
		OperationResult<bool> WriteConfiguration(Guid deviceUID);

		[OperationContract]
		OperationResult<string> GetInfo(Guid deviceUID);

		[OperationContract]
		OperationResult<List<FS2Journal>> ReadJournal(Guid deviceUID);

		[OperationContract]
		OperationResult<DeviceConfiguration> ReadConfiguration(Guid deviceUID);
	}
}