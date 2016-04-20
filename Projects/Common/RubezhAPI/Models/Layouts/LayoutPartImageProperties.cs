using System.Runtime.Serialization;
using Common;

namespace RubezhAPI.Models.Layouts
{
	[DataContract]
	public class LayoutPartImageProperties : LayoutPartReferenceProperties
	{
		[DataMember]
		public Stretch Stretch { get; set; }

		[DataMember]
		public ResourceType ImageType { get; set; }
	}
}