using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public abstract class FilterBase
	{
		[DataMember]
		public bool WithDeleted { get; set; }
	}
}
