using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS2Api;
using System.ServiceModel;
using FiresecAPI;
using FiresecAPI.Models;

namespace ServerFS2.Service
{
	[ServiceBehavior(MaxItemsInObjectGraph = Int32.MaxValue, UseSynchronizationContext = false,
	InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class FS2Contract : IFS2Contract
	{
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
	}
}