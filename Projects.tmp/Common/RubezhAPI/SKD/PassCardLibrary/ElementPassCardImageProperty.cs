using System;
using System.Runtime.Serialization;
using System.Windows.Media;
using RubezhAPI.Models;
using System.Xml.Serialization;

namespace RubezhAPI.SKD
{
	[DataContract]
	public class ElementPassCardImageProperty
	{
		public ElementPassCardImageProperty()
		{
			
		}

		[DataMember]
		public PassCardImagePropertyType PropertyType { get; set; }
		[DataMember]
		public Guid AdditionalColumnUID { get; set; }
		[DataMember]
		public Stretch Stretch { get; set; }
		[DataMember]
		public string Text { get; set; }

		[XmlIgnore]
		public Guid OrganisationUID { get; set; }

		public void UpdateZLayer()
		{
			
		}
	}
}