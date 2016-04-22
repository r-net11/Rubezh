using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Media;
using Infrastructure.Plans.Elements;

namespace FiresecAPI.Models.SKDDatabase
{
	[DataContract]
	public class PassCardTemplate : IElementBackground, IElementRectangle
	{
		public PassCardTemplate()
		{
			UID = Guid.NewGuid();
			Caption = "Шаблон пропуска";
			Width = 297;
			Height = 210;
			BackgroundColor = Colors.Transparent;
			IsVectorImage = false;
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
		}

		[DataMember]
		public string Caption { get; set; }
		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public Guid UID { get; set; }
		[DataMember]
		public double Width { get; set; }
		[DataMember]
		public double Height { get; set; }
		[DataMember]
		public Color BackgroundColor { get; set; }
		[DataMember]
		public byte[] BackgroundPixels { get; set; }
		[DataMember]
		public Guid? BackgroundImageSource { get; set; }
		[DataMember]
		public string BackgroundSourceName { get; set; }
		[DataMember]
		public bool IsVectorImage { get; set; }

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

		public bool AllowTransparent
		{
			get { return true; }
		}
	}
}
