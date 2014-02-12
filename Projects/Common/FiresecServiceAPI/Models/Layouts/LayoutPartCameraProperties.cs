using System;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace FiresecAPI.Models.Layouts
{
	[DataContract]
	public class LayoutPartCameraProperties : ILayoutProperties
	{
		[DataMember]
		public Guid SourceUID { get; set; }
	}
}