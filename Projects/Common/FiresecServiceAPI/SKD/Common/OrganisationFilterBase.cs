using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
    public abstract class OrganisationFilterBase : IsDeletedFilter, IAsyncFilter
	{
		[DataMember]
		public List<Guid> OrganisationUIDs { get; set; }

		[DataMember]
		public Guid UserUID { get; set; }

		public OrganisationFilterBase()
			: base()
		{
			OrganisationUIDs = new List<Guid>();
        }

        #region IAsyncFilterMembers
        [DataMember]
        public bool IsLoad { get; set; }

        [DataMember]
        public Guid ClientUID { get; set; }
        #endregion
    }

    public interface IAsyncFilter
    {
        bool IsLoad{ get; }
        Guid ClientUID { get; }
    }
}