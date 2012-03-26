using System;
using System.Collections.Generic;
using FiresecAPI.Models;
using System.Runtime.Serialization;

namespace GroupControllerModule.Models
{
    [DataContract]
    public class XDeviceLogic
    {
        public XDeviceLogic()
        {
            StateLogics = new List<StateLogic>();
        }

        [DataMember]
        public List<StateLogic> StateLogics { get; set; }
    }
}