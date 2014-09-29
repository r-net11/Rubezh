using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	public class XCode : XBase
	{
		public XCode()
		{
			Name = "Новый код";
		}

		[XmlIgnore]
		public override XBaseObjectType ObjectType { get { return XBaseObjectType.Code; } }

		public int Password { get; set; }
	}
}