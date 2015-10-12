using System;
using System.Runtime.Serialization;
using RubezhAPI.Models;
using Infrustructure.Plans.Elements;

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

		public Guid OrganisationUID { get; set; }

		public override void UpdateZLayer()
		{
			ZLayer = 70;
		}
	}
}