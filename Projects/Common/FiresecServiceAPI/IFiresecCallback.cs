using System.ServiceModel;
using FiresecAPI.Models;
using System.Collections.Generic;

namespace FiresecAPI
{
    public interface IFiresecCallback
    {
        [OperationContract(IsOneWay = true)]
        void Progress(int stage, string comment, int percentComplete, int bytesRW);

        [OperationContract(IsOneWay = true)]
        void DeviceStateChanged(List<DeviceState> deviceStates);

        [OperationContract(IsOneWay = true)]
        void DeviceParametersChanged(List<DeviceState> deviceStates);

        [OperationContract(IsOneWay = true)]
        void ZoneStateChanged(ZoneState zoneState);

        [OperationContract(IsOneWay = true)]
        void NewJournalRecord(JournalRecord journalRecord);
    }
}