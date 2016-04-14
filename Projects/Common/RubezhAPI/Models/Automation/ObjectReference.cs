using System;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public struct ObjectReference
	{
		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public ObjectType ObjectType { get; set; }

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			if (obj.GetType() != typeof(ObjectReference))
				return false;

			var objRef = (ObjectReference)obj;

			return objRef.ObjectType == this.ObjectType && objRef.UID == this.UID;
		}

		public override int GetHashCode()
		{
			return UID.GetHashCode();
		}

		public static bool operator ==(ObjectReference left, ObjectReference right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(ObjectReference left, ObjectReference right)
		{
			return !left.Equals(right);
		}
	}
}
