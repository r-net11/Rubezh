﻿using System.Runtime.Serialization;
using System;

namespace XFiresecAPI
{
	[DataContract]
	public class XPim : XBase
	{
		public override XBaseObjectType ObjectType { get { return XBaseObjectType.Pim; } }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public Guid MPTUID { get; set; }

		public override string PresentationName
		{
			get { return Name; }
		}
	}
}