﻿using System;
using System.Runtime.Serialization;

namespace RubezhAPI.Models.Layouts
{
	[DataContract]
	[KnownType(typeof(LayoutPartImageProperties))]
	[KnownType(typeof(LayoutPartPlansProperties))]
	[KnownType(typeof(LayoutPartReferenceProperties))]
	[KnownType(typeof(LayoutPartProcedureProperties))]
	[KnownType(typeof(LayoutPartTimeProperties))]
	[KnownType(typeof(LayoutPartTextProperties))]
	public class LayoutPart
	{
		public LayoutPart()
		{
		}

		[DataMember]
		public string Title { get; set; }
		[DataMember]
		public Guid UID { get; set; }
		[DataMember]
		public Guid DescriptionUID { get; set; }
		[DataMember]
		public ILayoutProperties Properties { get; set; }
	}
}