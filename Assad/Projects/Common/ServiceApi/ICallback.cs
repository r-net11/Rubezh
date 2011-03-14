using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace ServiceApi
{
    public interface ICallback
    {
        [OperationContract(IsOneWay = true)]
        void Notify(string message);

        [OperationContract(IsOneWay=true)]
        void ConfigurationChanged();

        [OperationContract(IsOneWay = true)]
        void DeviceChanged(Device device);

        [OperationContract(IsOneWay=true)]
        void ZoneChanged(Zone zone);
    }
}
