using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integration.Service.Entities
{
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
	public partial class zone
	{

		private zoneParam[] paramField;

		private int idxField;

		private int noField;

		private string nameField;

		private int idZonesField;

		private string descField;

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("param")]
		public zoneParam[] param
		{
			get
			{
				return this.paramField;
			}
			set
			{
				this.paramField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public int idx
		{
			get
			{
				return this.idxField;
			}
			set
			{
				this.idxField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public int no
		{
			get
			{
				return this.noField;
			}
			set
			{
				this.noField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string name
		{
			get
			{
				return this.nameField;
			}
			set
			{
				this.nameField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public int idZones
		{
			get
			{
				return this.idZonesField;
			}
			set
			{
				this.idZonesField = value;
			}
		}

		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string desc
		{
			get
			{
				return this.descField;
			}
			set
			{
				this.descField = value;
			}
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class zoneParam
	{

		private string nameField;

		private string typeField;

		private string valueField;

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string name
		{
			get
			{
				return this.nameField;
			}
			set
			{
				this.nameField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string type
		{
			get
			{
				return this.typeField;
			}
			set
			{
				this.typeField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string value
		{
			get
			{
				return this.valueField;
			}
			set
			{
				this.valueField = value;
			}
		}
	}
}
