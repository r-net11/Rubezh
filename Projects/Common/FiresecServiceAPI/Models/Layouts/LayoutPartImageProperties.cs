using Common;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace StrazhAPI.Models.Layouts
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