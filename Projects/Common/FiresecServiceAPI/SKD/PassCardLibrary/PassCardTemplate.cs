using Common;
using StrazhAPI.Plans.Elements;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Media;
using StrazhAPI.Models;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class PassCardTemplate : OrganisationElementBase, IElementBackground, IElementRectangle, IElementBorder
	{
		public PassCardTemplate()
		{
			UID = Guid.NewGuid();
			Caption = Resources.Language.SKD.PassCardLibrary.PassCardTemplate.PassCardTemplate_Caption;
			Width = 210;
			Height = 297;
			BackgroundColor = Colors.Transparent;
			BorderColor = Colors.Black;
			BorderThickness = 0;
			ImageType = ResourceType.Image;
			ClearElements();
		}

		public void ClearElements()
		{
			ElementRectangles = new List<ElementRectangle>();
			ElementEllipses = new List<ElementEllipse>();
			ElementTextBlocks = new List<ElementTextBlock>();
			ElementPolygons = new List<ElementPolygon>();
			ElementPolylines = new List<ElementPolyline>();
			ElementExtensions = new List<ElementBase>();
			ElementImageProperties = new List<ElementPassCardImageProperty>();
			ElementTextProperties = new List<ElementPassCardTextProperty>();
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
		public string BackgroundSourceName { get; set; }

		[DataMember]
		public ResourceType ImageType { get; set; }

		[DataMember]
		public List<ElementRectangle> ElementRectangles { get; set; }

		[DataMember]
		public List<ElementEllipse> ElementEllipses { get; set; }

		[DataMember]
		public List<ElementTextBlock> ElementTextBlocks { get; set; }

		[DataMember]
		public List<ElementPolygon> ElementPolygons { get; set; }

		[DataMember]
		public List<ElementPolyline> ElementPolylines { get; set; }

		[DataMember]
		public List<ElementBase> ElementExtensions { get; set; }

		[DataMember]
		public List<ElementPassCardImageProperty> ElementImageProperties { get; set; }

		[DataMember]
		public List<ElementPassCardTextProperty> ElementTextProperties { get; set; }

		public bool AllowTransparent
		{
			get { return true; }
		}
	}
}