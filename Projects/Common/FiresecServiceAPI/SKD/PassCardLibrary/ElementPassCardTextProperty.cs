using StrazhAPI.Plans.Elements;
using System;
using System.Runtime.Serialization;
using StrazhAPI.Models;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class ElementPassCardTextProperty : ElementTextBlock
	{
		public ElementPassCardTextProperty()
		{
		}

		public override ElementBase Clone()
		{
			ElementPassCardTextProperty elementBase = new ElementPassCardTextProperty();
			Copy(elementBase);
			return elementBase;
		}

		public override void Copy(ElementBase element)
		{
			base.Copy(element);
			((ElementPassCardTextProperty)element).PropertyType = PropertyType;
			((ElementPassCardTextProperty)element).AdditionalColumnUID = AdditionalColumnUID;
		}

		[DataMember]
		public PassCardTextPropertyType PropertyType { get; set; }

		[DataMember]
		public Guid AdditionalColumnUID { get; set; }

		public Guid OrganisationUID { get; set; }

		public override void UpdateZLayer()
		{
			ZLayer = 70;
		}
	}
}