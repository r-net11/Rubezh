using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Infrastructure.Models
{
    [DataContract]
    public class MultiLayoutCameraSettings
    {
        public MultiLayoutCameraSettings()
        {
            _2X2Collection = new Dictionary<string, Guid>();
            _1X7Collection = new Dictionary<string, Guid>();
            _3X3Collection = new Dictionary<string, Guid>();
            _4X4Collection = new Dictionary<string, Guid>();
            _6X6Collection = new Dictionary<string, Guid>();
        }

        [DataMember]
        public Dictionary<string, Guid> _2X2Collection;

        [DataMember]
        public Dictionary<string, Guid> _1X7Collection;

        [DataMember]
        public Dictionary<string, Guid> _3X3Collection;

        [DataMember]
        public Dictionary<string, Guid> _4X4Collection;

        [DataMember]
        public Dictionary<string, Guid> _6X6Collection;
    }
}
