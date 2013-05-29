using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using FiresecAPI.Models;
using FS2Api;

namespace FS2Client
{
	public partial class FS2
	{
		#region Main
		public List<FS2Callbac> Poll(Guid clientUID)
		{
			return SafeOperationCall(() => { return FS2Contract.Poll(clientUID); }, "Poll");
		}
		public void CancelPoll(Guid clientUID)
		{
			SafeOperationCall(() => { FS2Contract.CancelPoll(clientUID); }, "CancelPoll");
		}
		public void CancelProgress()
		{
			SafeOperationCall(() => { FS2Contract.CancelProgress(); }, "CanceProgress");
		}
		#endregion

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
			return SafeOperationCall(() => { return FS2Contract.SynchronizeTime(deviceUID); }, "SynchronizeTime");
		}
	}
}