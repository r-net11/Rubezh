using System;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;

namespace Infrustructure.Plans.Elements
{
	[DataContract]
	public abstract class ElementBase : IElementBackground
	{
		public ElementBase()
		{
			UID = Guid.NewGuid();
			BackgroundColor = Colors.White;
			BorderColor = Colors.Black;
			BorderThickness = 1;
			BackgroundPixels = null;
			BackgroundImageSource = null;
			BackgroundSourceName = null;
			IsVectorImage = false;
			IsLocked = false;
			IsHidden = false;
			SetDefault();
		}

		public virtual string Name
		{
			get { return GetType().FullName; }
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public Color BorderColor { get; set; }
		[DataMember]
		public double BorderThickness { get; set; }
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
		public int ZIndex { get; set; }
		[DataMember]
		public bool IsLocked { get; set; }
		[DataMember]
		public bool IsHidden { get; set; }

		public bool AllowTransparent
		{
			get { return false; }
		}

		public virtual int ZLayer
		{
			get { return 0; }
		}

		public Point Position
		{
			get
			{
				Rect rect = GetRectangle();
				return new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
			}
			set { SetPosition(value); }
		}
		public abstract ElementType Type { get; }
		public abstract Rect GetRectangle();
		protected abstract void SetPosition(Point point);
		public abstract ElementBase Clone();
		protected abstract void SetDefault();

		protected virtual void Copy(ElementBase element)
		{
			element.UID = UID;
			element.BorderColor = BorderColor;
			element.BorderThickness = BorderThickness;
			element.BackgroundColor = BackgroundColor;
			element.BackgroundImageSource = BackgroundImageSource;
			element.BackgroundSourceName = BackgroundSourceName;
			element.IsVectorImage = IsVectorImage;
			element.ZIndex = ZIndex;
			element.IsLocked = IsLocked;
			element.IsHidden = IsHidden;
		}
	}
}