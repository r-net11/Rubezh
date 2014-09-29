using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	public class XDirectionZone
	{
		public XDirectionZone()
		{
			StateBit = XStateBit.Fire1;
		}

		[XmlIgnore]
		public XZone Zone { get; set; }

		public Guid ZoneUID { get; set; }
		public XStateBit StateBit { get; set; }
	}
}