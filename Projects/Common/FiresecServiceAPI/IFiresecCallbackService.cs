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
		void DeviceStateChanged(List<DeviceState> deviceStates);

		[OperationContract(IsOneWay = true)]
		void DeviceParametersChanged(List<DeviceState> deviceStates);

		[OperationContract(IsOneWay = true)]
		void ZonesStateChanged(List<ZoneState> zoneStates);

		[OperationContract(IsOneWay = true)]
		void NewJournalRecords(List<JournalRecord> journalRecords);

		[OperationContract(IsOneWay = true)]
		void ConfigurationChanged();

		[OperationContract(IsOneWay = true)]
		void GetFilteredArchiveCompleted(IEnumerable<JournalRecord> journalRecords);

		[OperationContract]
		bool Progress(int stage, string comment, int percentComplete, int bytesRW);

		[OperationContract]
		Guid Ping();

		[OperationContract(IsOneWay = true)]
		void Notify(string message);
	}
}