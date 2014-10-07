using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class GKCode : GKBase
	{
		public GKCode()
		{
			Name = "Новый код";
		}

		[XmlIgnore]
		public override GKBaseObjectType ObjectType { get { return GKBaseObjectType.Code; } }

		[DataMember]
		public int Password { get; set; }
	}
}