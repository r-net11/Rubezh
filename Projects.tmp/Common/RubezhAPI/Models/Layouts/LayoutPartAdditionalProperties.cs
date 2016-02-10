using System.Runtime.Serialization;

namespace RubezhAPI.Models.Layouts
{
	[DataContract]
	public class LayoutPartAdditionalProperties : ILayoutProperties
	{
		[DataMember]
		public bool IsVisibleBottomPanel { get; set; }
	}
}