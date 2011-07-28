using System.Collections.Generic;
using System.ServiceModel;
using FiresecAPI.Models;

namespace FiresecAPI
{
    public interface IFiresecCallback
    {
        [OperationContract(IsOneWay = true)]
        void DeviceStateChanged(string deviceId);

        [OperationContract(IsOneWay = true)]
        void DeviceParametersChanged(string deviceId);

        [OperationContract(IsOneWay = true)]
        void ZoneStateChanged(string zoneNo);

        [OperationContract(IsOneWay = true)]
        void NewJournalItem(JournalItem journalItem);
    }
}
