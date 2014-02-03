using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI
{
    [DataContract]
    public class PositionFilter
    {
        [DataMember]
        public List<Guid> Uids;

        public bool HasUids
        {
            get { return Uids.Count > 0; }
        }

        public PositionFilter()
        {
            Uids = new List<Guid>();
        }
    }
}
