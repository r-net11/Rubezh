using RubezhAPI.Plans.Elements;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.Models
{
	[DataContract]
	public class ElementRectangle : ElementBaseRectangle, IPrimitive
	{
		public ElementRectangle()
			: base()
		{
			PresentationName = "Прямоугольник";
		}

		[DataMember]
		public bool ShowTooltip { get; set; }

		#region IPrimitive Members

		[XmlIgnore]
		public virtual Primitive Primitive
		{
			get { return RubezhAPI.Plans.Elements.Primitive.Rectangle; }
		}

		#endregion
	}
}