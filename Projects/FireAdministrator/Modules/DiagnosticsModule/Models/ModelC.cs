using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DiagnosticsModule.Models_CC
{
	public class ModelC
	{
		public string Name { get; set; }
		[XmlText]
		public string Description { get; set; }

		string _nonAutoProperty;
		public string NonAutoProperty
		{
			get { return _nonAutoProperty; }
			set { _nonAutoProperty = value; }
		}
	}
}