using Common;
using System;
using System.Runtime.Serialization;
using System.Windows;
using System.Xml.Serialization;

namespace StrazhAPI.Plans.Elements
{
	[DataContract]
	public abstract class ElementBase : IElementBackground, IElementBorder, IElementBase
	{
		public ElementBase()
		{
			UID = Guid.NewGuid();
			UpdateZLayer();
			BackgroundColor = Colors.White;
			BorderColor = Colors.Black;
			BorderThickness = 1;
			BackgroundImageSource = null;
			BackgroundSourceName = null;
			ImageType = ResourceType.Image;
			IsLocked = false;
			IsHidden = false;
		}

		[XmlIgnore]
		public virtual string Name
		{
			get { return GetType().FullName; }
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string PresentationName { get; set; }

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
		public int ZIndex { get; set; }

		[DataMember]
		public bool IsLocked { get; set; }

		[DataMember]
		public bool IsHidden { get; set; }

		[XmlIgnore]
		public bool AllowTransparent
		{
			get { return false; }
		}

		[DataMember]
		public int ZLayer { get; set; }

		[XmlIgnore]
		public Point Position
		{
			get
			{
				Rect rect = GetRectangle();
				return new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
			}
			set { SetPosition(value); }
		}

		[XmlIgnore]
		public abstract ElementType Type { get; }

		public abstract Rect GetRectangle();

		protected abstract void SetPosition(Point point);

		public abstract ElementBase Clone();

		public virtual void Copy(ElementBase element)
		{
			element.UID = UID;
			element.BorderColor = BorderColor;
			element.BorderThickness = BorderThickness;
			element.BackgroundColor = BackgroundColor;
			element.BackgroundImageSource = BackgroundImageSource;
			element.BackgroundSourceName = BackgroundSourceName;
			element.ImageType = ImageType;
			element.ZIndex = ZIndex;
			element.IsLocked = IsLocked;
			element.IsHidden = IsHidden;
		}

		public virtual void UpdateZLayer()
		{
			ZLayer = 0;
		}
	}
}