using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using DiagnosticsModule.Models_AAA;
using DiagnosticsModule.Models_CC;

namespace DiagnosticsModule.Models
{
	[XmlRoot("XmlModelB")]
	public class ModelB : ModelAAA
	{
		public ModelB()
		{
			ModelItems = new List<ModelItem>();
			PrivateProperty = "Private Hello Property";
			PrivateField = "Private Hello Field";
		}

		public string Description { get; set; }
		private string PrivateProperty { get; set; }
		private string PrivateField;

		[XmlIgnore]
		public string PublicField;

		public ModelC ModelC1 { get; set; }
		public ModelC ModelC2 { get; set; }

		[XmlArrayItem("CustomModelItems", typeof(ModelItem))]
		[XmlArray("CustomModelItem")]
		public List<ModelItem> ModelItems { get; set; }

		public virtual string VirtualPrioerty { get; set; }
		public string SetProperty
		{
			set { ;}
		}
		public string GetProperty
		{
			get { return "Get"; }
		}
	}
}