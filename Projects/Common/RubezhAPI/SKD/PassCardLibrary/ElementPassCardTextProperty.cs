using RubezhAPI.Models;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.SKD
{
	[DataContract]
	public class ElementPassCardTextProperty : ElementTextBlock
	{
		public ElementPassCardTextProperty()
		{
		}

		[DataMember]
		public PassCardTextPropertyType PropertyType { get; set; }
		[DataMember]
		public Guid AdditionalColumnUID { get; set; }

		[XmlIgnore]
		public Guid OrganisationUID { get; set; }

		public override void UpdateZLayer()
		{
			ZLayer = 70;
		}
	}
}