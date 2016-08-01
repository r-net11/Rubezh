using Common;
using StrazhAPI.Models;
using StrazhAPI.Plans.Elements;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	[KnownType(typeof(PassCardTemplateSide))]
	public class PassCardTemplate : OrganisationElementBase, IElementRectangle
	{
		public PassCardTemplate()
		{
			UID = Guid.NewGuid();
			Width = 210;
			Height = 297;
		}

		[DataMember]
		public PassCardTemplateSide Front { get; set; }

		[DataMember]
		public PassCardTemplateSide Back { get; set; }

		[DataMember]
		public bool IsDualTemplateEnabled { get; set; }

		[DataMember]
		public string Caption { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public double Width { get; set; }

		[DataMember]
		public double Height { get; set; }
	}
}