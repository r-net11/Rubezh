using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.Models.Layouts
{
	[DataContract]
	public class LayoutPart
	{
		public LayoutPart()
		{
		}

		[DataMember]
		public string Title { get; set; }
		[DataMember]
		public Guid UID { get; set; }
		[DataMember]
		public Guid DescriptionUID { get; set; }
		[DataMember]
		[XmlElement("LayoutPartImageProperties", Type = typeof(LayoutPartImageProperties))]
		[XmlElement("LayoutPartPlansProperties", Type = typeof(LayoutPartPlansProperties))]
		[XmlElement("LayoutPartReferenceProperties", Type = typeof(LayoutPartReferenceProperties))]
		[XmlElement("LayoutPartProcedureProperties", Type = typeof(LayoutPartProcedureProperties))]
		[XmlElement("LayoutPartTimeProperties", Type = typeof(LayoutPartTimeProperties))]
		[XmlElement("LayoutPartTextProperties", Type = typeof(LayoutPartTextProperties))]
		[XmlElement("LayoutPartAdditionalProperties", Type = typeof(LayoutPartAdditionalProperties))]
		[XmlElement("LayoutPartJournalProperties", Type = typeof(LayoutPartJournalProperties))]
		public object Properties { get; set; }
	}
}