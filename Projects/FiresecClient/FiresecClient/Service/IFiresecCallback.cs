using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace FiresecClient
{
    public interface IFiresecCallback
    {
        [OperationContract(IsOneWay = true)]
        void NewEvent(string name);
    }
}
