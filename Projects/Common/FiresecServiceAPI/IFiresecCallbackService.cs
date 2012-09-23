using System;
using System.Collections.Generic;
using System.ServiceModel;
using FiresecAPI.Models;

namespace FiresecAPI
{
	[ServiceContract(SessionMode = SessionMode.Required)]
	public interface IFiresecCallbackService
	{
        [OperationContract(IsOneWay = true)]
        void NewJournalRecords(List<JournalRecord> journalRecords);

		[OperationContract(IsOneWay = true)]
		void ConfigurationChanged();

		[OperationContract(IsOneWay = true)]
		void GetFilteredArchiveCompleted(IEnumerable<JournalRecord> journalRecords);

		[OperationContract]
		Guid Ping();

		[OperationContract(IsOneWay = true)]
		void Notify(string message);
	}
}