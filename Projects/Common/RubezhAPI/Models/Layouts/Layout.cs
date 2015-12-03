using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace RubezhAPI.Models.Layouts
{
	[DataContract]
	public class Layout
	{
		static Guid _noLayoutUID = new Guid("AB973D0C-292B-4E47-BFD6-C9C1910F5557");
		public static Guid NoLayoutUID { get { return _noLayoutUID; } }
		public Layout()
		{
			UID = Guid.NewGuid();
			Users = new List<Guid>();
			Parts = new List<LayoutPart>();
			HostNameOrAddressList = new List<string>();
			SplitterSize = 4;
			SplitterColor = Colors.Transparent;
			IsRibbonEnabled = true;
		}
		public Layout(List<string> otherCaptions) : this()
		{
			Caption = CreateNewCaption(otherCaptions);
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
		string CreateNewCaption(List<string> otherCaptions)
		{
			var defaultCaption = "Новый макет";
			var counter = 1;
			var newCaption = defaultCaption;
			while (otherCaptions.Contains(newCaption))
			{
				newCaption = string.Format("{0}({1})", defaultCaption, counter);
				counter++;
			}
			return newCaption;
		}
	}
}