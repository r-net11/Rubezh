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
			Users = new List<GKUser>();
		}

		[DataMember]
		public bool HasError { get; set; }

		[DataMember]
		public string Error { get; set; }

		[DataMember]
		public CallbackOperationResultType CallbackOperationResultType { get; set; }

		[DataMember]
		public List<GKUser> Users { get; set; }

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