using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public abstract class SKDIsDeletedModel : SKDModelBase
	{
		[DataMember]
		public bool IsDeleted { get; set; }

		[DataMember]
		public DateTime RemovalDate { get; set; }
	}

	[DataContract]
	public abstract class SKDModelBase
	{
		public SKDModelBase()
		{
			UID = Guid.NewGuid();
		}

		[DataMember]
		public Guid UID { get; set; }
	}

	public interface IOrganisationElement
	{
		Guid UID { get; set; }

		string Name { get; set; }

		string Description { get; set; }

		Guid OrganisationUID { get; set; }

		bool IsDeleted { get; set; }

		DateTime RemovalDate { get; set; }
	}

	public class SKDModelComparer<T> : IEqualityComparer<T>
		where T : SKDModelBase
	{
		#region IEqualityComparer<T> Members

		public bool Equals(T x, T y)
		{
			return x.UID == y.UID;
		}

		public int GetHashCode(T obj)
		{
			return obj.UID.GetHashCode();
		}

		#endregion IEqualityComparer<T> Members
	}
}