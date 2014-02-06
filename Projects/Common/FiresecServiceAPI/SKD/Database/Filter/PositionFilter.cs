using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
    [DataContract]
	public class PositionFilter : FilterBase
    {
        [DataMember]
		public List<Guid> Uids { get; set; }

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
