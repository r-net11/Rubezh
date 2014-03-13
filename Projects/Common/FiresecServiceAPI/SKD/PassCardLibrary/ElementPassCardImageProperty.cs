using System;
using System.Runtime.Serialization;
using System.Windows.Media;
using FiresecAPI.Models;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.SKD.PassCardLibrary
{
	[DataContract]
	public class ElementPassCardImageProperty : ElementRectangle
	{
		public ElementPassCardImageProperty()
		{
		}

		public override ElementBase Clone()
		{
			ElementPassCardImageProperty elementBase = new ElementPassCardImageProperty();
			Copy(elementBase);
			return elementBase;
		}
		public override void Copy(ElementBase element)
		{
			base.Copy(element);
			((ElementPassCardImageProperty)element).PropertyType = PropertyType;
			((ElementPassCardImageProperty)element).AdditionalColumn = AdditionalColumn;
			((ElementPassCardImageProperty)element).Stretch = Stretch;
		}

		[DataMember]
		public PassCardImagePropertyType PropertyType { get; set; }
		[DataMember]
		public Guid AdditionalColumn { get; set; }
		[DataMember]
		public Stretch Stretch { get; set; }

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