using System;
using System.Runtime.Serialization;
using System.Windows.Media;
using FiresecAPI.Models;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.SKD
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

		public Guid OrganisationUID { get; set; }

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