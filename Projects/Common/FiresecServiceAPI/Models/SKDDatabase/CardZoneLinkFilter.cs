using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
    [DataContract]
    public class CardZoneLinkFilter
    {
        [DataMember]
		public List<Guid> Uids { get; set; }
		
		public CardZoneLinkFilter()
        {
            Uids = new List<Guid>();
        }
    }
}
