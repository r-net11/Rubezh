using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFiresecAPI
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