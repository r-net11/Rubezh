using System.ServiceModel;
using FiresecAPI.Models;

namespace FiresecAPI
{
    public interface IFiresecCallback
    {
        [OperationContract(IsOneWay = true)]
        void DeviceStateChanged(DeviceState deviceState);

        [OperationContract(IsOneWay = true)]
        void DeviceParametersChanged(DeviceState deviceState);

        [OperationContract(IsOneWay = true)]
        void ZoneStateChanged(ZoneState zoneState);

        [OperationContract(IsOneWay = true)]
        void NewJournalRecord(JournalRecord journalRecord);
    }
}