using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DiagnosticsModule.Models_AAA
{
	public class ModelAAA
	{
		public int Count { get; set; }
		public string Name { get; set; }

		[XmlElement("ElementProperty2")]
		public string ElementProperty { get; set; }

		[XmlAttribute("AttributeProperty2")]
		public string AttributeProperty { get; set; }
	}
}