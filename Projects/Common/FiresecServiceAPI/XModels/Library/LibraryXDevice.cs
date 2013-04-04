using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
    [DataContract]
    public class LibraryXDevice
    {
        public LibraryXDevice()
        {
			UID = Guid.NewGuid();
			IsAlternative = false;
            XStates = new List<LibraryXState>();
            var libraryXState = new LibraryXState()
            {
                XStateClass = XStateClass.No,
            };
            XStates.Add(libraryXState);
        }

        public XDriver Driver { get; set; }

		[DataMember]
		public Guid UID { get; set; }

        [DataMember]
        public Guid XDriverId { get; set; }

		[DataMember]
		public bool IsAlternative { get; set; }

		[DataMember]
		public string AlternativeName { get; set; }

        [DataMember]
        public List<LibraryXState> XStates { get; set; }
    }
}