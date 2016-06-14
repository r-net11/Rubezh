using StrazhAPI.Plans.Elements;
using System;
using System.Runtime.Serialization;
using System.Windows.Media;
using StrazhAPI.Models;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class ElementPassCardImageProperty : ElementRectangle
	{
		public ElementPassCardImageProperty()
		{
			Stretch = Stretch.Fill;
		}

		public override ElementBase Clone()
		{
			var elementBase = new ElementPassCardImageProperty();
			Copy(elementBase);
			return elementBase;
		}

		public override void Copy(ElementBase element)
		{
			base.Copy(element);
			((ElementPassCardImageProperty)element).PropertyType = PropertyType;
			((ElementPassCardImageProperty)element).AdditionalColumnUID = AdditionalColumnUID;
			((ElementPassCardImageProperty)element).Stretch = Stretch;
			((ElementPassCardImageProperty)element).Text = Text;
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