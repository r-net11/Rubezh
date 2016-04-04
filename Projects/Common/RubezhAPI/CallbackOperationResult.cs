using RubezhAPI.GK;
using RubezhAPI.Journal;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RubezhAPI
{
	[DataContract]
	public class CallbackOperationResult
	{
		public CallbackOperationResult()
		{
			Users = new List<GKUser>();
			JournalItems = new List<JournalItem>();
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
		public Guid ClientUid { get; set; }

		/// <summary>
		/// Пользователь, от имени которого выполнилась операция
		/// </summary>
		[DataMember]
		public string UserName { get; set; }
	}

	public enum CallbackOperationResultType
	{
		RewriteUsers,
		WriteConfiguration,
		ReadConfigurationFromGKFile,
		GetArchivePage,
		GetJournal,
		GetPmfUsers,
		GetGKUsers
	}
}