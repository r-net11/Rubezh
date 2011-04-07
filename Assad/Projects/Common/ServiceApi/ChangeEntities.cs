using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ServiceApi
{
    [DataContract]
    public class ChangeEntities
    {
        [DataMember]
        public bool StateChanged { get; set; }

        [DataMember]
        public bool StatesChanged { get; set; }

        [DataMember]
        public bool ParameterChanged { get; set; }

        [DataMember]
        public bool VisibleParameterChanged { get; set; }

        public void Reset()
        {
            StateChanged = false;
            StatesChanged = false;
            ParameterChanged = false;
            VisibleParameterChanged = false;
        }
    }
}
