using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;

namespace FiresecAPI.GK
{
	[DataContract]
	public class XCode : XBase
	{
		public XCode()
		{
			Name = "Новый код";
		}

		public override XBaseObjectType ObjectType { get { return XBaseObjectType.Code; } }

		[DataMember]
		public int Password { get; set; }
	}
}