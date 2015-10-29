using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;
using Common;

namespace Infrustructure.Plans.Elements
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
		[DataMember]
		public bool AllowMultipleVizualization { get; set; }

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

		public ElementBase Clone()
		{
			Type type2create = this.GetType();
			ElementBase newElement = (ElementBase)Activator.CreateInstance(type2create);
			this.Copy(newElement);
			return newElement;
		}

		public void Copy(ElementBase target)
		{
			PropertyInfo[] sourceProperties = this.GetType().GetProperties();
			PropertyInfo[] targetProperties = target.GetType().GetProperties();

			foreach (PropertyInfo sourceProperty in sourceProperties)
			{
				PropertyInfo targetProperty = targetProperties
					.Where(property => property.GetSetMethod() != null
					&& property.Name == sourceProperty.Name
					&& property.PropertyType == sourceProperty.PropertyType)
					.FirstOrDefault();
				if (targetProperty != null)
				{
					object propertyValue = sourceProperty.GetValue(this, null);
					// Sometimes there are Point Collections in the Objects.
					// New Instance should be created and Points should be copied from the Source.
					if (sourceProperty.PropertyType == typeof(PointCollection))
					{
						PointCollection pointCollection = (PointCollection)propertyValue;
						PointCollection clone = new PointCollection(pointCollection);
						targetProperty.SetValue(target, clone, null);
					}
					else
						// Other Properties are copied by their Value:
						targetProperty.SetValue(target, propertyValue, null);
				}
			}
		}
		public virtual void UpdateZLayer()
		{
			ZLayer = 0;
		}
	}
}