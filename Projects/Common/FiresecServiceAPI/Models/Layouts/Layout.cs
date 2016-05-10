using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace StrazhAPI.Models.Layouts
{
	[DataContract]
	public class Layout
	{
		public Layout()
		{
			UID = Guid.NewGuid();
			Users = new List<Guid>();
			Parts = new List<LayoutPart>();
			HostNameOrAddressList = new List<string>();
			Caption = Resources.Language.Models.Layouts.Layout.Caption;
			SplitterSize = 4;
			SplitterColor = Colors.Transparent;
			IsRibbonEnabled = true;
		}

		[DataMember]
		public bool IsRibbonEnabled { get; set; }

		[DataMember]
		public List<Guid> Users { get; set; }

		[DataMember]
		public List<LayoutPart> Parts { get; set; }

		[DataMember]
		public List<string> HostNameOrAddressList { get; set; }

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Caption { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public string Content { get; set; }

		[DataMember]
		public int SplitterSize { get; set; }

		[DataMember]
		public Color SplitterColor { get; set; }

		[DataMember]
		public int BorderThickness { get; set; }

		[DataMember]
		public Color BorderColor { get; set; }

		[DataMember]
		public Color BackgroundColor { get; set; }

		[DataMember]
		public int Padding { get; set; }

		public LayoutPart GetLayoutPart(Guid uid)
		{
			return Parts.FirstOrDefault(item => item.UID == uid);
		}

		public LayoutPart GetLayoutPartByType(Guid typeUID)
		{
			return Parts.FirstOrDefault(item => item.DescriptionUID == typeUID);
		}
	}
}