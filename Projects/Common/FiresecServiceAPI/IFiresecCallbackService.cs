using System.ServiceModel;
using FiresecAPI.Models;
using System;
using System.Collections.Generic;

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
		void ZoneStateChanged(ZoneState zoneState);

		[OperationContract(IsOneWay = true)]
		void NewJournalRecord(JournalRecord journalRecord);

		[OperationContract(IsOneWay = true)]
		void ConfigurationChanged();

		[OperationContract(IsOneWay = false)]
		bool Progress(int stage, string comment, int percentComplete, int bytesRW);

		[OperationContract(IsOneWay = false)]
		Guid Ping();
	}
}