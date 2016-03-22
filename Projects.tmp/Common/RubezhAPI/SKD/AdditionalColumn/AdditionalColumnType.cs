﻿using System.Runtime.Serialization;

namespace RubezhAPI.SKD
{
	[DataContract]
	public class AdditionalColumnType : OrganisationElementBase, IOrganisationElement
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public AdditionalColumnDataType DataType { get; set; }

		[DataMember]
		public PersonType PersonType { get; set; }

		[DataMember]
		public bool IsInGrid { get; set; }
	}
}