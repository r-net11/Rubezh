using Common;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.SKD
{
	[DataContract]
	public class ElementPassCardImageProperty : ElementRectangle
	{
		public ElementPassCardImageProperty()
		{
			Stretch = Stretch.Fill;
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

		[XmlIgnore]
		public override Primitive Primitive
		{
			get { return Primitive.NotPrimitive; }
		}
		public override void UpdateZLayer()
		{
			ZLayer = 70;
		}
	}
}