using RubezhAPI.GK;
using System;
using RubezhAPI.Journal;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace RubezhAPI
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

		/// <summary>
		/// Прибор, с которого читаются пользователи
		/// </summary>
		[DataMember]
		public Guid DeviceUID { get; set; }

		[DataMember]
		public string FileName { get; set; }

		[DataMember]
		public List<JournalItem> JournalItems { get; set; }

		[DataMember]
		public int PageNo { get; set; }
	}

	public enum CallbackOperationResultType
	{
		GetGKUsers,
		RewriteUsers,
		WriteConfiguration,
		ReadConfigurationFromGKFile,
		GetPmfUsers,
		GetArchivePage,
		GetJournal,
	}
}