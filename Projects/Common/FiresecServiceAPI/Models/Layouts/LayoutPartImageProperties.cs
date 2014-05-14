using System;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace FiresecAPI.Models.Layouts
{
	[DataContract]
	public class LayoutPartImageProperties : ILayoutProperties
	{
		[DataMember]
		public Stretch Stretch { get; set; }

		[DataMember]
		public Guid SourceUID { get; set; }

		[DataMember]
		public bool IsVectorImage { get; set; }
	}
}