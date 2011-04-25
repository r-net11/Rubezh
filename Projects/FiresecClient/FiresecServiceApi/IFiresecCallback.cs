using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace FiresecServiceApi
{
    public interface IFiresecCallback
    {
        [OperationContract(IsOneWay = true)]
        void NewEventsAvailable(int eventMask, string obj);
    }
}
