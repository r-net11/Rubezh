﻿using System;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class Phone : OrganisationElementBase
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string NumberString { get; set; }

		[DataMember]
		public Guid? DepartmentUid { get; set; }
	}
}