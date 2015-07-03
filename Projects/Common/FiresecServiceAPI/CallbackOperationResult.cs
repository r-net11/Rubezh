using FiresecAPI.GK;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class CallbackOperationResult
	{
		public CallbackOperationResult()
		{
			Users = new OperationResult<List<GKUser>>();
			Result = new OperationResult<bool>();
		}

		[DataMember]
		public CallbackOperationResultType CallbackOperationResultType { get; set; }

		[DataMember]
		public OperationResult<List<GKUser>> Users { get; set; }

		[DataMember]
		public OperationResult<bool> Result { get; set; }

		[DataMember]
		public string FileName { get; set; }
	}

	public enum CallbackOperationResultType
	{
		GetAllUsers,
		RewriteUsers,
		WriteConfiguration,
		ReadConfigurationFromGKFile,
	}
}