using System;
using System.Runtime.Serialization;
using RubezhAPI.Models;
using System.Xml.Serialization;

namespace RubezhAPI.SKD
{
	[DataContract]
	public class ElementPassCardTextProperty
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

		public void UpdateZLayer()
		{
			
		}
	}
}