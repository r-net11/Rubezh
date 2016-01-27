using Common;
using RubezhAPI.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Media;
using System.Xml.Serialization;

namespace RubezhAPI.SKD
{
	[DataContract]
	public class PassCardTemplate : OrganisationElementBase, IOrganisationElement
	{
		public PassCardTemplate()
		{
			UID = Guid.NewGuid();
			Caption = "Шаблон пропуска";
			Width = 210;
			Height = 300;
			BorderThickness = 2;
			ImageType = ResourceType.Image;
			ClearElements();
		}
		public void ClearElements()
		{
			ElementImageProperties = new List<ElementPassCardImageProperty>();
			ElementTextProperties = new List<ElementPassCardTextProperty>();
			PassCardImages = new List<PassCardImage>();
		}

		[DataMember]
		public string Caption { get; set; }
		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public double Width { get; set; }
		[DataMember]
		public double Height { get; set; }
		[DataMember]
		public Color BorderColor { get; set; }
		[DataMember]
		public double BorderThickness { get; set; }
		[DataMember]
		public Color BackgroundColor { get; set; }
		[DataMember]
		public Guid? BackgroundImageSource { get; set; }
		[DataMember]
		public Guid? BackgroundSVGImageSource { get; set; }
		[DataMember]
		public string BackgroundSourceName { get; set; }
		[DataMember]
		public ResourceType ImageType { get; set; }

		[DataMember]
		public List<ElementPassCardImageProperty> ElementImageProperties { get; set; }
		[DataMember]
		public List<ElementPassCardTextProperty> ElementTextProperties { get; set; }
		[DataMember]
		public List<PassCardImage> PassCardImages { get; set; }

		[XmlIgnore]
		public bool AllowTransparent
		{
			get { return true; }
		}

		[XmlIgnore]
		public string Name
		{
			get { return Caption; }
			set { Caption = value; }
		}
	}

	[DataContract]
	public class PassCardImage
	{
		[DataMember]
		public Guid ImageUID { get; set; }

		[DataMember]
		public byte[] Image { get; set; }
	}
}